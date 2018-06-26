using DealerManagementSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Xml.Linq;

namespace DealerManagementSystem.Utils
{
    public class BusinessLogic
    {
        const int _storeId = 1; //TODO რომელ საწყობს უყუროს

        #region Catalog
        public List<GroupProduct> GetGroupProducts()
        {
            using (NTContext _nt = new NTContext())
            {
                List<GroupProduct> rootGroupProducts = _nt.GetList<GroupProduct>("SELECT * FROM book.GroupProducts WHERE parentid = @parentId AND name NOT LIKE N'*%'", new SqlParameter() { ParameterName = "@parentId", Value = "11" });
                if(rootGroupProducts != null)
                {
                    rootGroupProducts.ForEach(rgp =>
                    {
                        rgp.Children = FillGroupProductChildren(rgp.Id);
                    });
                }

                return rootGroupProducts;
            }
        }

        List<GroupProduct> FillGroupProductChildren(int id)
        {
            using (NTContext _nt = new NTContext())
            {
                List<GroupProduct> groupProducts = _nt.GetList<GroupProduct>("SELECT * FROM book.GroupProducts WHERE parentId = @parentId AND name NOT LIKE N'*%'", new SqlParameter() { ParameterName = "@parentId", Value = id });

                if(groupProducts != null)
                {
                    groupProducts.ForEach(gp =>
                    {
                        gp.Children = FillGroupProductChildren(gp.Id);
                    });
                }

                return groupProducts;
            }
        }

        public List<Product> GetProductsByGroupId(int groupId)
        {
            using (NTContext _nt = new NTContext())
            {
                string groupPath = _nt.GetString("SELECT g.path FROM book.GroupProducts AS g WHERE g.id = @groupId AND g.name NOT LIKE N'*%'", new SqlParameter { ParameterName = "@groupId", Value = groupId });

                List<Product> products = _nt.GetList<Product>(@"DECLARE @store_id INT=" + _storeId + @"; WITH rest_data(product_id, rest) AS
                                                                (
                                                                  SELECT fl.product_id, ISNULL(SUM(fl.amount * CASE fl.is_order WHEN 1 THEN -1 WHEN 0 THEN fl.coeff END),0)  AS rest
                                                                  FROM book.Products AS p
                                                                  INNER JOIN doc.ProductsFlow AS fl ON fl.product_id=p.id
                                                                  INNER JOIN doc.GeneralDocs AS g ON g.id=fl.general_id
                                                                  WHERE g.tdate <= GETDATE() AND ISNULL(g.is_deleted,0)=0  AND fl.is_order IN(0,1)  AND fl.is_expense=0 AND @store_id=fl.store_id AND p.path LIKE '0#1#10%'
                                                                  GROUP BY fl.product_id
                                                                )
                                                                SELECT t1.id, t1.path, t1.code, " + (GetLanguageCookieValue() == "ge" ? "t1.comment AS name," : "t1.name,") + @" 0 AS quantity, t0.rest AS stock, ROUND(pp.manual_val, 2) AS price, c.code AS currency, t1.usr_column_516 AS manufacturer FROM rest_data AS t0 INNER JOIN book.Products AS t1 ON t1.id=t0.product_id
                                                                INNER JOIN book.ProductPrices AS pp
                                                                ON pp.product_id = t1.id AND pp.price_id = @priceId
                                                                INNER JOIN book.Currencies AS c
                                                                ON pp.manual_currency_id = c.id
                                                                WHERE t1.path LIKE N'%" + groupPath + "%' AND t1.path IN(SELECT gp.path FROM book.GroupProducts AS gp WHERE gp.path LIKE N'%" + groupPath + "%' AND gp.name NOT LIKE N'*%')", new SqlParameter { ParameterName = "@priceId", Value = GetContragentSpecialPrice().Id });

                if (products != null)
                {
                    products.ForEach(p =>
                    {
                        int? contragentShoppingCartQuantity = GetShoppingCartQuantityByContragentIdAndProductId(GetCurrentUserId(), p.Id);
                        
                        p.Ordered = contragentShoppingCartQuantity.HasValue ? contragentShoppingCartQuantity.Value : 0;
                        p.Stock -= p.Ordered;
                    });
                }

                return products;
            }
        }

        public double? GetShoppingCartAmountSumByContragentId(int contragentId)
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetScalar<double>(@"SELECT SUM(sc.quantity * sc.price) AS price_sum
                                               FROM shop.ShoppingCarts AS sc
                                               WHERE sc.contragent_id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = contragentId });
            }
        }

        double? GetProductPriceByProductId(int productId)
        {
            using (NTContext _nt = new NTContext())
            {
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@productId", Value = productId },
                    new SqlParameter { ParameterName = "@priceId", Value = GetContragentSpecialPrice().Id }
                };

                return _nt.GetScalar<double>(@"SELECT pp.manual_val AS price
                                               FROM book.Products AS p
                                               INNER JOIN book.ProductPrices AS pp
                                               ON pp.product_id = p.id AND pp.price_id = @priceId
                                               WHERE p.id = @productId", sqlParams);
            }
        }

        public bool AddToShoppingCart(ShoppingCart shoppingCart)
        {
            using (NTContext _nt = new NTContext())
            {
                double? productPrice = GetProductPriceByProductId(shoppingCart.ProductId);

                double productQuantityRest = GetProductQuantityRestIncludingShoppingCartByProductId(shoppingCart.ProductId);

                if (shoppingCart.Quantity > 0 && shoppingCart.Quantity <= productQuantityRest)
                {
                    ShoppingCart ntShoppingCart = GetShoppingCartByContragentIdAndProductId(GetCurrentUserId(), shoppingCart.ProductId);

                    if (ntShoppingCart != null) //update
                    {
                        SqlParameter[] sqlParams = new SqlParameter[]
                        {
                            new SqlParameter { ParameterName = "@shoppingCartId", Value = ntShoppingCart.Id },
                            new SqlParameter { ParameterName = "@quantity", Value = ntShoppingCart.Quantity + shoppingCart.Quantity }
                        };

                        return _nt.ExecuteSql(@"UPDATE shop.ShoppingCarts SET quantity = @quantity WHERE id = @shoppingCartId", sqlParams) > 0;
                    }
                    else //insert
                    {
                        SqlParameter[] sqlParams = new SqlParameter[]
                        {
                            new SqlParameter { ParameterName = "@productId", Value = shoppingCart.ProductId },
                            new SqlParameter { ParameterName = "@quantity", Value = shoppingCart.Quantity },
                            new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() },
                            new SqlParameter { ParameterName = "@price", Value = productPrice.HasValue ? productPrice.Value : 0 }
                        };

                        return _nt.ExecuteSql(@"INSERT INTO shop.ShoppingCarts VALUES(@productId, @quantity, @contragentId, @price)", sqlParams) > 0;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        KeyValuePair<int, double> GetProductRestOriginal(int product_id, int store_id, DateTime toDate)
        {
            using (NTContext _nt = new NTContext())
            {
                string id_string = product_id.ToString();
                string sql = @"SELECT p.id, (SELECT ISNULL(SUM(a.amount*a.coeff),0) 
                             FROM   doc.ProductsFlow a 
                             INNER JOIN doc.GeneralDocs g ON g.id=a.general_id
                             WHERE  a.is_order = 0 AND ISNULL(g.is_deleted,0) = 0 AND a.is_expense=0 AND g.tdate<='" + toDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + @"' AND a.product_id = p.id
                             AND a.store_id = CASE " + store_id + @" WHEN 0 THEN a.store_id ELSE " + store_id + @" END) as rest
                             FROM book.Products AS p WHERE p.id = " + id_string + ";";

                return _nt.GetDictionary<int, double>(sql).FirstOrDefault();
            }
        }

        int? GetShoppingCartQuantityByProductId(int productId)
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetScalar<int>("SELECT SUM(sc.quantity) FROM shop.ShoppingCarts AS sc WHERE sc.product_id = @productId", new SqlParameter { ParameterName = "@productId", Value = productId });
            }
        }

        int? GetShoppingCartQuantityByContragentIdAndProductId(int contragentId, int productId)
        {
            using (NTContext _nt = new NTContext())
            {
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@contragentId", Value = contragentId },
                    new SqlParameter { ParameterName = "@productId", Value = productId }
                };

                return _nt.GetScalar<int>("SELECT sc.quantity FROM shop.ShoppingCarts AS sc WHERE sc.contragent_id = @contragentId AND sc.product_id = @productId", sqlParams);
            }
        }        

        double GetProductQuantityRestIncludingShoppingCartByProductId(int productId)
        {
            int? shoppingCartQuantity = GetShoppingCartQuantityByProductId(productId);

            return GetProductRestOriginal(productId, _storeId, DateTime.Now).Value - (shoppingCartQuantity.HasValue ? shoppingCartQuantity.Value : 0);
        }

        ShoppingCart GetShoppingCartByContragentIdAndProductId(int contragentId, int productId)
        {
            using (NTContext _nt = new NTContext())
            {
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@contragentId", Value = contragentId },
                    new SqlParameter { ParameterName = "@productId", Value = productId }
                };

                return _nt.GetList<ShoppingCart>("SELECT * FROM shop.ShoppingCarts AS sc WHERE sc.contragent_id = @contragentId AND sc.product_id = @productId", sqlParams).FirstOrDefault();
            }
        }

        public Price GetContragentSpecialPrice()
        {
            using (NTContext _nt = new NTContext())
            {
                //int? priceId = _nt.GetScalar<int>("SELECT price_id FROM book.ContragentAgreements AS ca WHERE ca.contragent_id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = contragentId });

                //return priceId.HasValue ? priceId.Value : 12/*3*/; //დეფაულტად თუ კონტრაგენტს ხელშეკრულება არ აქვს დავაბრუნო საცალო ფასის ტიპის (GEL) Id = 3 ან საბითუმო(USD) Id = 12. TODO change

                return _nt.GetList<Price>("SELECT id, name FROM book.Prices WHERE id = @id", new SqlParameter { ParameterName = "@id", Value = Convert.ToInt32(GetCurrentContragent().Col14_556) }).FirstOrDefault();
            }
        }

        public List<Product> SearchProducts(string searchPhrase)
        {
            using (NTContext _nt = new NTContext())
            {
                List<Product> products = _nt.GetList<Product>(@"DECLARE @store_id INT=" + _storeId + @"; WITH rest_data(product_id, rest) AS
                                                                (
                                                                  SELECT fl.product_id, ISNULL(SUM(fl.amount * CASE fl.is_order WHEN 1 THEN -1 WHEN 0 THEN fl.coeff END),0)  AS rest
                                                                  FROM book.Products AS p
                                                                  INNER JOIN doc.ProductsFlow AS fl ON fl.product_id=p.id
                                                                  INNER JOIN doc.GeneralDocs AS g ON g.id=fl.general_id
                                                                  INNER JOIN book.GroupProducts AS gp ON gp.id = p.group_id AND gp.name NOT LIKE N'*%'
                                                                  WHERE g.tdate <= GETDATE() AND ISNULL(g.is_deleted,0)=0  AND fl.is_order IN(0,1)  AND fl.is_expense=0 AND @store_id=fl.store_id AND p.path LIKE '0#1#10%'
                                                                  GROUP BY fl.product_id
                                                                )
                                                                SELECT t1.id, t1.code, " + (GetLanguageCookieValue() == "ge" ? "t1.comment AS name," : "t1.name,") + @" 0 AS quantity, t0.rest AS stock, ROUND(pp.manual_val, 2) AS price, c.code AS currency, t1.usr_column_516 AS manufacturer FROM rest_data AS t0 INNER JOIN book.Products AS t1 ON t1.id=t0.product_id
                                                                INNER JOIN book.ProductPrices AS pp
                                                                ON pp.product_id = t1.id AND pp.price_id = @priceId
                                                                INNER JOIN book.GroupProducts AS gp ON gp.id = t1.group_id AND gp.name NOT LIKE N'*%'
                                                                INNER JOIN book.Currencies AS c
                                                                ON pp.manual_currency_id = c.id
                                                                WHERE (" + (GetLanguageCookieValue() == "ge" ? "t1.comment" : "t1.name") + @" LIKE N'%" + searchPhrase + @"%' OR t1.code ='" + searchPhrase + @"')", new SqlParameter { ParameterName = "@priceId", Value = GetContragentSpecialPrice().Id });

                if (products != null)
                {
                    products.ForEach(p =>
                    {
                        int? contragentShoppingCartQuantity = GetShoppingCartQuantityByContragentIdAndProductId(GetCurrentUserId(), p.Id);
                        
                        p.Ordered = contragentShoppingCartQuantity.HasValue ? contragentShoppingCartQuantity.Value : 0;
                        p.Stock -= p.Ordered;
                    });
                }

                return products;
            }
        }
        #endregion



        #region Shopping Cart
        public List<ContragentAddress> GetContragentAddresses()
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetList<ContragentAddress>("SELECT id, contragent_id AS contragentId, address FROM book.ContragentAddresses WHERE contragent_id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() });
            }
        }

        public List<Product> GetContragentShoppingCartProducts()
        {
            List<int> contragentShoppingCartProductIds = GetShoppingCartProductIdsByContragentId(GetCurrentUserId());

            if(contragentShoppingCartProductIds != null)
            {
                string cscpis = string.Join(",", contragentShoppingCartProductIds.ConvertAll(Convert.ToString).ToArray());

                using (NTContext _nt = new NTContext())
                {
                    List<Product> products = _nt.GetList<Product>(@"DECLARE @store_id INT=" + _storeId + @"; WITH rest_data(product_id, rest) AS
                                                                    (
                                                                      SELECT fl.product_id, ISNULL(SUM(fl.amount * CASE fl.is_order WHEN 1 THEN -1 WHEN 0 THEN fl.coeff END),0)  AS rest
                                                                      FROM book.Products AS p
                                                                      INNER JOIN doc.ProductsFlow AS fl ON fl.product_id=p.id
                                                                      INNER JOIN doc.GeneralDocs AS g ON g.id=fl.general_id
                                                                      INNER JOIN book.GroupProducts AS gp ON gp.id = p.group_id AND gp.name NOT LIKE N'*%'
                                                                      WHERE g.tdate <= GETDATE() AND ISNULL(g.is_deleted,0)=0  AND fl.is_order IN(0,1)  AND fl.is_expense=0 AND @store_id=fl.store_id AND p.path LIKE '0#1#10%'
                                                                      GROUP BY fl.product_id
                                                                    )
                                                                    SELECT t1.id, t1.code, " + (GetLanguageCookieValue() == "ge" ? "t1.comment AS name," : "t1.name,") + @" 0 AS quantity, t0.rest AS stock, ROUND(pp.manual_val, 2) AS price, c.code AS currency, t1.usr_column_516 AS manufacturer FROM rest_data AS t0 INNER JOIN book.Products AS t1 ON t1.id=t0.product_id
                                                                    INNER JOIN book.ProductPrices AS pp
                                                                    ON pp.product_id = t1.id AND pp.price_id = @priceId
                                                                    INNER JOIN book.GroupProducts AS gp ON gp.id = t1.group_id AND gp.name NOT LIKE N'*%'
                                                                    INNER JOIN book.Currencies AS c
                                                                    ON pp.manual_currency_id = c.id
                                                                    WHERE t1.id IN(" + cscpis + @")", new SqlParameter { ParameterName = "@priceId", Value = GetContragentSpecialPrice().Id });

                    if(products != null)
                    {
                        products.ForEach(p =>
                        {
                            int? contragentShoppingCartQuantity = GetShoppingCartQuantityByContragentIdAndProductId(GetCurrentUserId(), p.Id);
                            
                            p.Ordered = contragentShoppingCartQuantity.HasValue ? contragentShoppingCartQuantity.Value : 0;
                            p.Stock -= p.Ordered;
                            p.Total = p.Stock + p.Ordered;
                            p.TotalSum = Math.Round(p.Ordered * (p.Price.HasValue ? p.Price.Value : 0), 2);
                        });
                    }

                    return products;
                }
            }
            else
            {
                return null;
            }
        }

        List<int> GetShoppingCartProductIdsByContragentId(int contragentId)
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetList<int>("SELECT sc.product_id FROM shop.ShoppingCarts AS sc WHERE sc.contragent_id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = contragentId });
            }
        }

        public bool UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            using (NTContext _nt = new NTContext())
            {
                SqlParameter[] sqlParamsPP = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@productId", Value = shoppingCart.ProductId },
                    new SqlParameter { ParameterName = "@priceId", Value = GetContragentSpecialPrice().Id }
                };

                double? productPrice = _nt.GetScalar<double>(@"SELECT pp.manual_val AS price
                                                               FROM book.Products AS p
                                                               INNER JOIN book.ProductPrices AS pp
                                                               ON pp.product_id = p.id AND pp.price_id = @priceId
                                                               WHERE p.id = @productId", sqlParamsPP);

                int? contragentShoppingCartQuantity = GetShoppingCartQuantityByContragentIdAndProductId(GetCurrentUserId(), shoppingCart.ProductId);

                if(contragentShoppingCartQuantity.HasValue)
                {
                    double productQuantityRest = GetProductQuantityRestIncludingShoppingCartByProductId(shoppingCart.ProductId) + contragentShoppingCartQuantity.Value;

                    if (shoppingCart.Quantity > 0 && shoppingCart.Quantity <= productQuantityRest)
                    {
                        ShoppingCart ntShoppingCart = GetShoppingCartByContragentIdAndProductId(GetCurrentUserId(), shoppingCart.ProductId);

                        if (ntShoppingCart != null) //update
                        {
                            SqlParameter[] sqlParams = new SqlParameter[]
                            {
                                new SqlParameter { ParameterName = "@shoppingCartId", Value = ntShoppingCart.Id },
                                new SqlParameter { ParameterName = "@quantity", Value = shoppingCart.Quantity }
                            };

                            return _nt.ExecuteSql(@"UPDATE shop.ShoppingCarts SET quantity = @quantity WHERE id = @shoppingCartId", sqlParams) > 0;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool RemoveShoppingCart(int productId)
        {
            int contragentId = GetCurrentUserId();

            ShoppingCart contragentShoppingCart = GetShoppingCartByContragentIdAndProductId(contragentId, productId);

            if(contragentShoppingCart != null)
            {
                using (NTContext _nt = new NTContext())
                {
                    SqlParameter[] sqlParams = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@contragentId", Value = contragentId },
                        new SqlParameter { ParameterName = "@productId", Value = productId }
                    };

                    return _nt.ExecuteSql("DELETE FROM shop.ShoppingCarts WHERE contragent_id = @contragentId AND product_id = @productId", sqlParams) > 0;
                }
            }
            else
            {
                return false;
            }
        }

        List<ShoppingCart> GetShoppingCartsByContragentId(int contragentId)
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetList<ShoppingCart>("SELECT sc.id, sc.product_id AS productId, sc.quantity, sc.contragent_id AS contragentId, sc.price FROM shop.ShoppingCarts AS sc WHERE sc.contragent_id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = contragentId });
            }
        }

        public bool PlaceOrder(int contragentAddressId)
        {
            using (TransactionScope _ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = new TimeSpan(0, 10, 0) }))
            {
                using (NTContext _nt = new NTContext())
                {
                    bool result = false;

                    List<ShoppingCart> contragentShoppingCarts = GetShoppingCartsByContragentId(GetCurrentUserId());

                    if (!OrderFromBuyer(contragentShoppingCarts, contragentAddressId))
                    {
                        return false;
                    }

                    result = MoveFromShoppingCartsToOrders();

                    if (result)
                    {
                        _ts.Complete();
                    }

                    return result;
                }
            }
        }

        bool MoveFromShoppingCartsToOrders()
        {
            using (NTContext _nt = new NTContext())
            {
                bool result = false;

                List<ShoppingCart> contragentShoppingCarts = GetShoppingCartsByContragentId(GetCurrentUserId());

                foreach(ShoppingCart csc in contragentShoppingCarts)
                {
                    SqlParameter[] sqlParams = new SqlParameter[]
                    {
                        new SqlParameter { ParameterName = "@productId", Value = csc.ProductId },
                        new SqlParameter { ParameterName = "@quantity", Value = csc.Quantity },
                        new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() },
                        new SqlParameter { ParameterName = "@price", Value = csc.Price },
                        new SqlParameter { ParameterName = "@orderDate", Value = DateTime.Now }
                    };

                    bool orderSaved = _nt.ExecuteSql("INSERT INTO shop.Orders VALUES(@productId, @quantity, @contragentId, @price, @orderDate)", sqlParams) > 0;

                    if (orderSaved)
                    {
                        result = RemoveShoppingCart(csc.ProductId);

                        if (!result)
                        {
                            return false;
                        }
                    }
                }

                return result;
            }
        }
        #endregion



        #region Product Details
        public Product GetProductDetails(int productId)
        {
            using (NTContext _nt = new NTContext())
            {
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@productId", Value = productId },
                    new SqlParameter { ParameterName = "@priceId", Value = GetContragentSpecialPrice().Id }
                };

                Product product = _nt.GetList<Product>(@"DECLARE @store_id INT=" + _storeId + @"; WITH rest_data(product_id, rest) AS
                                                        (
                                                            SELECT fl.product_id, ISNULL(SUM(fl.amount * CASE fl.is_order WHEN 1 THEN -1 WHEN 0 THEN fl.coeff END),0)  AS rest
                                                            FROM book.Products AS p
                                                            INNER JOIN doc.ProductsFlow AS fl ON fl.product_id=p.id
                                                            INNER JOIN doc.GeneralDocs AS g ON g.id=fl.general_id
                                                            INNER JOIN book.GroupProducts AS gp ON gp.id = p.group_id AND gp.name NOT LIKE N'*%'
                                                            WHERE g.tdate <= GETDATE() AND ISNULL(g.is_deleted,0)=0  AND fl.is_order IN(0,1)  AND fl.is_expense=0 AND @store_id=fl.store_id AND p.path LIKE '0#1#10%'
                                                            GROUP BY fl.product_id
                                                        )
                                                        SELECT t1.id, t1.code, " + (GetLanguageCookieValue() == "ge" ? "t1.comment AS name," : "t1.name,") + @" 0 AS quantity, t0.rest AS stock, ROUND(pp.manual_val, 2) AS price, c.code AS currency, t1.usr_column_516 AS manufacturer FROM rest_data AS t0 INNER JOIN book.Products AS t1 ON t1.id=t0.product_id
                                                        INNER JOIN book.ProductPrices AS pp
                                                        ON pp.product_id = t1.id AND pp.price_id = @priceId
                                                        INNER JOIN book.GroupProducts AS gp ON gp.id = t1.group_id AND gp.name NOT LIKE N'*%'
                                                        INNER JOIN book.Currencies AS c
                                                        ON pp.manual_currency_id = c.id
                                                        WHERE t1.id = @productId", sqlParams).FirstOrDefault();

                if(product != null)
                {
                    int? contragentShoppingCartQuantity = GetShoppingCartQuantityByContragentIdAndProductId(GetCurrentUserId(), product.Id);
                    
                    product.Ordered = contragentShoppingCartQuantity.HasValue ? contragentShoppingCartQuantity.Value : 0;
                    product.Stock -= product.Ordered;
                    product.Total = product.Stock + product.Ordered;
                    product.TotalSum = Math.Round(product.Ordered * (product.Price.HasValue ? product.Price.Value : 0), 2);

                    product.Images = _nt.GetList<ProductImage>("SELECT pi.img AS image FROM book.ProductImages AS pi WHERE pi.product_id = @productId", new SqlParameter { ParameterName = "@productId", Value = productId });
                }

                return product;
            }
        }
        #endregion



        #region Profile Info
        public Contragent GetCurrentContragent()
        {
            using (NTContext _nt = new NTContext())
            {
                Contragent contragent = _nt.GetList<Contragent>(@"SELECT id, usr_column_542 AS userName, code, name, tel AS phone, address,
                                                                  usr_column_543 AS col1_543,
                                                                  usr_column_544 AS col2_544,
                                                                  usr_column_545 AS col3_545,
                                                                  usr_column_546 AS col4_546,
                                                                  usr_column_547 AS col5_547,
                                                                  usr_column_548 AS col6_548,

                                                                  usr_column_549 AS col7_549_C4,
                                                                  usr_column_550 AS col8_550_C4,

                                                                  usr_column_551 AS col9_551,
                                                                  usr_column_552 AS col10_552,
                                                                  usr_column_553 AS col11_553,
                                                                  usr_column_554 AS col12_554,
                                                                  usr_column_555 AS col13_555,
                                                                  usr_column_556 AS col14_556,

                                                                  usr_column_557 AS col7_557_C5,
                                                                  usr_column_558 AS col8_558_C5,

                                                                  usr_column_559 AS col7_559_C6,
                                                                  usr_column_560 AS col8_560_C6,

                                                                  usr_column_561 AS col7_561_C7,
                                                                  usr_column_562 AS Col8_562_C7

                                                                  FROM book.Contragents WHERE id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() }).FirstOrDefault();

                if(contragent != null)
                {
                    contragent.ContragentDiscounts = _nt.GetList<ContragentDiscount>("SELECT p.name, ca.discount FROM book.ContragentAgreements AS ca INNER JOIN book.Prices AS p ON ca.price_id = p.id WHERE ca.contragent_id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = contragent.Id });
                }

                return contragent;
            }
        }

        public bool ChangePassword(string oldPwd, string newPwd)
        {
            if(oldPwd != newPwd)
            {
                using (NTContext _nt = new NTContext())
                {
                    string op = _nt.GetString("SELECT usr_column_541 FROM book.Contragents WHERE id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() });

                    if (op == oldPwd)
                    {
                        SqlParameter[] sqlParams = new SqlParameter[]
                        {
                            new SqlParameter { ParameterName = "@newPwd", Value = newPwd },
                            new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() }
                        };

                        return _nt.ExecuteSql("UPDATE book.Contragents SET usr_column_541 = @newPwd WHERE id = @contragentId", sqlParams) > 0;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        #endregion



        #region FinaOrders
        public List<FinaOrder> GetFinaOrders()
        {
            using (NTContext _nt = new NTContext())
            {
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@tdate", Value = DateTime.Now },
                    new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() },
                    new SqlParameter { ParameterName = "@currency", Value = GetContragentCurrency().Id }
                };

                List<FinaOrder> finaOrders = _nt.GetList<FinaOrder>(@"DECLARE @date AS DATETIME = @tdate
                                                                    DECLARE @curr AS INT = @currency
                                                                    ;WITH   contragents(id, account, account2) AS(SELECT id, account, account2 FROM book.contragents WHERE id = @contragentId),
                                                                                            debt(id, debet, credit)AS
                                                                                            (
                                                                                                        SELECT r.id, r.debit, r.credit
                                                                                                        FROM (
                                                                                                        SELECT c.id,
                                                                                                        CAST((SELECT ISNULL(SUM(e.amount * CASE @curr WHEN 0 THEN e.rate ELSE 1 END ),0) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id WHERE (0=@curr OR @curr=e.currency_id) AND e.a1=c.id AND (e.debit_acc =  c.account OR e.debit_acc =  c.account2)) AS DEC(38,2)) AS debit,
                                                                                                            CAST((SELECT ISNULL(SUM(e.amount * CASE @curr WHEN 0 THEN e.rate ELSE 1 END ),0) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id WHERE (0=@curr OR @curr=e.currency_id) AND e.b1=c.id AND (e.credit_acc =  c.account OR e.credit_acc =  c.account2))AS DEC(38,2)) AS credit
                                                                                                        FROM contragents AS c)
                                                                                                        AS r
                                                                                                        --WHERE  r.debit-r.credit &gt; 0
                                                                                            ),
                                                                                            flow(num, id, general_id, due_date, debit, dafaruli) AS
                                                                                            (
                                                                                            SELECT ROW_NUMBER()OVER(ORDER BY r.id, r.tdate) AS num, r.id, r.general_id, r.tdate, CAST(ISNULL(r.deb,0) AS DEC(38,5)) AS deb, r.dafaruli
                                                                                            FROM(
                                                                                                        select c.id, g.id AS general_id, pd.id AS deadliene_id,  COALESCE(pd.tdate, g.tdate) AS tdate, COALESCE(pd.amount * CASE @curr WHEN 0 THEN g.rate ELSE 1 END, SUM(e.amount * CASE @curr WHEN 0 THEN e.rate ELSE 1 END)) AS deb, d.credit AS dafaruli
                                                                                                        FROM debt AS d
                                                                                                        INNER JOIN contragents AS c ON c.id=d.id
                                                                                                        INNER JOIN doc.entries AS e ON e.a1=c.id AND (e.debit_acc =  c.account OR e.debit_acc =  c.account2)
                                                                                                        INNER JOIN doc.GeneralDocs AS g ON g.id=e.general_id
                                                                                                        LEFT JOIN doc.PaymentDeadlines AS pd ON pd.general_id=g.id
                                                                                                        --WHERE  COALESCE(pd.tdate,g.tdate) &lt; @date
                                                                            WHERE (0=@curr OR @curr=e.currency_id )
                                                                                                        GROUP BY  c.id,  g.id, pd.id, COALESCE(pd.tdate,g.tdate),  pd.amount * CASE @curr WHEN 0 THEN g.rate ELSE 1 END, d.credit
                                                                                                        ) AS r
                                                                                            ),
                                                                                            flow2(num, id, general_id, due_date, overdue_days, debit, credit, rest) AS
                                                                                            (
                                                                                            SELECT ROW_NUMBER()OVER(ORDER BY r.num) AS num, r.id, r.general_id, r.due_date,  CASE WHEN r.dafaruli-r.runing < 0 THEN DATEDIFF(dd, @date,r.due_date ) ELSE 0 END AS overdue, r.debit, CASE WHEN r.dafaruli- r.runing > 0 THEN r.debit ELSE CASE WHEN r.debit - ABS(r.dafaruli-r.runing) < 0 THEN 0 ELSE r.debit - ABS(r.dafaruli-r.runing)END END AS credit, r.dafaruli-r.runing
                                                                                            FROM(
                                                                                            SELECT  num, id,  general_id, due_date, debit, dafaruli, SUM(debit) OVER(PARTITION BY id ORDER BY num) AS runing 
                                                                                            FROM flow
                                                                                            ) AS r
                                                                                            --WHERE r.debt &lt; 0
                                                                                            ),
                                                                                            flow3(num, id, general_id, due_date, overdue_days, debit, credit, rest) AS
                                                                                            (
                                                                                            SELECT           num, id, general_id, due_date, overdue_days AS overdue_days, debit, credit, /*rest*/ debit-credit AS rest
                                                                                            FROM flow2 WHERE flow2.rest < 0          
                                                                                            )
 
 
                                                                    SELECT g.id, g.tdate, g.doc_num AS docNum, g.doc_type AS docType, g.status_id AS statusId, g.ref_id AS refId, po.transp_end_place AS address, f.due_date AS dueDate, f.overdue_days AS overdueDays, f.debit, f.credit, f.rest, po.waybill_status AS waybillStatus
                                                                    FROM flow3 AS f
                                                                    INNER JOIN doc.generaldocs AS g ON g.id=f.general_id
                                                                    INNER JOIN book.Contragents AS c ON c.id=f.id
                                                                    INNER JOIN doc.ProductOut AS po ON po.general_id = g.id AND g.status_id != 3

                                                                    UNION

                                                                    SELECT g.id, g.tdate, g.doc_num AS docNum, g.doc_type AS docType, g.status_id AS statusId, g.ref_id AS refId, co.transp_end_place AS address, NULL AS dueDate, NULL AS overdueDays, CAST(g.amount AS DECIMAL(38,2)) AS debit, NULL AS credit, CAST(0 AS FLOAT) AS rest, 0 AS waybillStatus
                                                                    FROM doc.GeneralDocs AS g
                                                                    INNER JOIN doc.CustomerOrders AS co ON co.general_id = g.id
                                                                    WHERE g.doc_type = 8 AND g.param_id1 = @contragentId AND g.tdate <= @tdate AND g.status_id != 3
                                                                    ORDER BY g.tdate", sqlParams);

                finaOrders.ForEach(fo =>
                {
                    if(fo.WaybillStatus != null)
                    {
                        if(fo.WaybillStatus == -1)
                        {
                            fo.StatusId = 2;
                        }

                        if(fo.WaybillStatus == 1)
                        {
                            fo.StatusId = 3;
                        }

                        if(fo.WaybillStatus == 2)
                        {
                            fo.StatusId = 4;
                        }
                    }
                    else
                    {
                        if(fo.StatusId != 2)
                        {
                            fo.StatusId = 1;
                        }
                    }

                    if (fo.OverdueDays < 0)
                    {
                        fo.StatusId = 5;
                    }
                });

                return finaOrders.Where(fo => (fo.DocType == 8 && fo.StatusId == 1) || fo.DocType == 21).ToList();
            }
        }

        public XElement GetFinaOrderInvoiceXml(int generalId)
        {
            using (NTContext _nt = new NTContext())
            {
                int? refGeneralId = _nt.GetScalar<int>("SELECT ref_id FROM doc.GeneralDocs WHERE id = @generalId", new SqlParameter { ParameterName = "@generalId", Value = generalId });

                Company company = GetCompany();
                Contragent contragent = GetCurrentContragent();
                List<InvoiceItem> invoiceItems = _nt.GetList<InvoiceItem>(@"SELECT  
                                                                        ROW_NUMBER() OVER(ORDER BY b.id) AS indexNumber,
                                                                        p.name AS productName, 
                                                                        p.code AS productCode, 
                                                                        u.full_name AS unitName,
                                                                        a.amount/T.coeff AS quantity,
                                                                        a.price*T.coeff/(1- a.discount_percent/100) AS price,
                                                                        a.price/(1- a.discount_percent/100) *  a.amount AS totalAmount,
                                                                        a.price/(1- a.discount_percent/100) *  a.amount * a.discount_percent/100 AS discounted_val,
                                                                        a.discount_percent,
                                                                        b.tdate, 
                                                                        (CASE WHEN b.doc_type=8 THEN b.doc_num ELSE f.invoice_num END) AS invoiceNum,  
                                                                        f.invoice_term AS invoiceTerm, 
                                                                        ba.bank_name AS bankName, 
                                                                        ba.bank_code AS bankCode,
                                                                        ba.account,     
                                                                        g.name as contragent_name,
                                                                        cur.code AS currencyCode,
                                                                        g.code as contragent_code, 
                                                                        f.transp_end_place as contragent_address, 
                                                                        g.tel as contragent_tel,
                                                                        (SELECT TOP 1 name FROM book.Companies) AS companyName, 
                                                                        (SELECT TOP 1 info FROM book.Companies) AS companyInfo 
                                                                        FROM doc.ProductsFlow a 
                                                                        INNER JOIN doc.GeneralDocs b ON a.general_id = b.id 
                                                                        INNER JOIN book.Currencies AS cur ON b.currency_id=cur.id
                                                                        INNER JOIN doc.CustomerOrders f ON a.general_id = f.general_id 
                                                                        INNER JOIN book.Contragents g ON  b.param_id1 = g.id 
                                                                        INNER JOIN book.Stores st ON  b.param_id2 = st.id
                                                                        LEFT JOIN (SELECT y.id, x.name as bank_name, x.code as bank_code, y.account  FROM book.CompanyAccounts y INNER JOIN book.Banks x ON y.bank_id = x.id INNER JOIN book.Currencies c ON y.currency_id = c.id)  ba ON ba.id=f.invoice_bank_id    
                                                                        INNER JOIN book.Products AS p ON p.id=a.product_id
                                                                        INNER JOIN book.Units AS u ON u.id=a.unit_id
                                                                            CROSS APPLY 
                                                                        ( 
		                                                                    SELECT ISNULL(MIN(amount),1) AS coeff  FROM book.ProductUnits WHERE product_id=a.product_id AND unit_id=a.unit_id 
                                                                        ) AS T       
                                                                        WHERE a.visible=1 AND b.id = @generalId", new SqlParameter { ParameterName = "@generalId", Value = refGeneralId == 0 ? generalId : refGeneralId });

                InvoiceItem invoiceItem = invoiceItems.FirstOrDefault();

                XElement element = new XElement("root",
                    new XElement("companyName", company.Name),
                    new XElement("companyCode", company.Code),
                    new XElement("companyTel", company.Tel),
                    new XElement("companyInfo", company.Info),
                    new XElement("companyChief", company.Chief),
                    new XElement("contragentName", contragent.Name),
                    new XElement("contragentCode", contragent.Code),
                    new XElement("contragentAddress", contragent.Address),
                    new XElement("tdate", invoiceItem == null ? null : invoiceItem.Tdate.Value.ToString("dd/MM/yyyy")),
                    new XElement("invoiceNum", invoiceItem == null ? 0 : invoiceItem.InvoiceNum),
                    new XElement("invoiceTerm", invoiceItem == null ? "" : invoiceItem.InvoiceTerm),
                    new XElement("bankName", invoiceItem == null ? "" : invoiceItem.BankName),
                    new XElement("bankCode", invoiceItem == null ? "" : invoiceItem.BankCode),
                    new XElement("account", invoiceItem == null ? "" : invoiceItem.Account),
                    new XElement("quantitySum", invoiceItem == null ? 0 : Math.Round(invoiceItems.Sum(x => x.Quantity), 2)),
                    new XElement("priceSum", invoiceItem == null ? 0 : Math.Round(invoiceItems.Sum(x => x.Price), 2)),
                    new XElement("currencyCode", invoiceItem == null ? "" : invoiceItem.CurrencyCode),
                    new XElement("totalAmountSum", invoiceItem == null ? 0 : Math.Round(invoiceItems.Sum(x => x.TotalAmount), 2)),
                    new XElement("items", invoiceItems.Select(ii =>
                        new XElement("item",
                            new XElement("indexNumber", ii.IndexNumber),
                            new XElement("productName", ii.ProductName),
                            new XElement("unitName", ii.UnitName),
                            new XElement("quantity", ii.Quantity),
                            new XElement("price", ii.Price),
                            new XElement("totalAmount", ii.TotalAmount)
                        )
                    ))
                );

                return element;
            }
        }
        #endregion



        #region History
        public KeyValuePair<double?, List<History>> GetHistory(string dateFrom, string dateTo, int currencyId)
        {
            using (NTContext _nt = new NTContext())
            {
                Contragent contragent = _nt.GetList<Contragent>("SELECT id, account, account2 FROM book.Contragents WHERE id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = GetCurrentUserId() }).FirstOrDefault();
                
                string sqlStartRest = @"SELECT (r.debit - r.credit) AS rest 
                                        FROM (
                                        SELECT c.id, 
                                        CAST((SELECT SUM(e.amount) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id AND g.tdate < @dateFrom AND e.currency_id=@CURRENCY WHERE  e.a1=c.id AND (e.debit_acc =  c.account OR e.debit_acc =  c.account2)) AS DEC(38,2)) AS debit,
                                        CAST((SELECT SUM(e.amount) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id AND g.tdate < @dateFrom AND e.currency_id=@CURRENCY WHERE  e.b1=c.id AND (e.credit_acc =  c.account OR e.credit_acc =  c.account2))AS DEC(38,2)) AS credit
                                        FROM book.Contragents AS c
                                        WHERE c.id = @contragentId) AS r";

                SqlParameter[] sqlParamsRest = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@contragentId", Value = contragent.Id },
                    new SqlParameter { ParameterName = "@dateFrom", Value = dateFrom },
                    new SqlParameter { ParameterName = "@CURRENCY", Value = currencyId }
                };
                double? startRest = _nt.GetScalar<double>(sqlStartRest, sqlParamsRest);
                double? originalRest = startRest;

                string sqlBase = @"SELECT gd.id, gd.doc_type AS docType, dt.tag, gd.tdate, gd.doc_num AS docNum, dt.name AS implementationOf, gd.waybill_num AS waybillNum,
                                   ROUND(SUM( CASE WHEN (e.debit_acc=@acc OR e.debit_acc=@acc2 ) AND e.a1=@PARAMID THEN e.amount ELSE 0 END ),2) AS amountIn,
                                   ROUND(SUM( CASE WHEN (e.credit_acc=@acc OR e.credit_acc=@acc2) AND e.b1=@PARAMID THEN e.amount ELSE 0 END ),2) AS amountOut
                                   FROM doc.Entries e INNER JOIN  doc.GeneralDocs gd On gd.id= e.general_id
                                   INNER JOIN config.DocTypes dt ON gd.doc_type = dt.id
                                   WHERE ISNULL(gd.is_deleted,0)=0 AND gd.tdate>=@TDATE AND gd.tdate<=@TDATE2 AND e.currency_id=@CURRENCY

                                   AND ( ( (e.debit_acc=@acc OR e.debit_acc=@acc2 ) AND e.a1 =@PARAMID) OR ((e.credit_acc=@acc OR e.credit_acc=@acc2 ) AND e.b1 =@PARAMID))
                                   GROUP BY gd.id, dt.tag,gd.tdate, gd.doc_num, gd.waybill_num, dt.name, gd.doc_type
                                   ORDER BY gd.tdate";

                SqlParameter[] sqlParamsBase = new SqlParameter[]
                {
                        new SqlParameter("@PARAMID", System.Data.SqlDbType.Int) { Value = contragent.Id },
                        new SqlParameter("@acc", System.Data.SqlDbType.VarChar) { Value = contragent.Account },
                        new SqlParameter("@acc2", System.Data.SqlDbType.VarChar) { Value = contragent.Account2 },
                        new SqlParameter("@TDATE", System.Data.SqlDbType.DateTime) { Value = dateFrom + " 00:00:00.000" },
                        new SqlParameter("@TDATE2", System.Data.SqlDbType.DateTime) { Value = dateTo + " 23:59:59.997" },
                        new SqlParameter("@CURRENCY", System.Data.SqlDbType.Int) { Value = currencyId }
                };

                List<History> histories = _nt.GetList<History>(sqlBase, sqlParamsBase);

                histories.ForEach(h =>
                {
                    if (startRest.HasValue)
                    {
                        h.Rest = startRest.Value;
                    }

                    if (h.AmountIn != 0)
                    {
                        h.Rest += h.AmountIn;
                        startRest = h.Rest;
                    }
                    else
                    {
                        h.Rest -= h.AmountOut;
                        startRest = h.Rest;
                    }

                    h.Rest = Math.Round(h.Rest, 2);
                });

                return new KeyValuePair<double?, List<History>>(originalRest, histories);
            }
        }

        public XElement GetHistoryWaybillXml(int generalId)
        {
            using (NTContext _nt = new NTContext())
            {
                //Database document                
                string sqlQuery = "SELECT  e.vat,e.product_nm AS product_name,e.product_code, e.unit_nm AS unit_name,a.amount AS quantity ,a.price*b.rate AS price," +
                " CASE WHEN NULLIF(b.waybill_num, '') IS NULL  THEN b.tdate ELSE f.transport_begin_date END  AS tdate, " +
                " f.reciever_IdNum, " +
                " f.reciever_name," +
                " f.responsable_person," +
                " f.responsable_person_num," +
                " f.responsable_person_date," +
                " f.sender_IdNum, " +
                " f.sender_name, " +
                " f.transporter_name, " +
                " f.transport_model, " +
                " f.transport_number, " +
                " f.transporter_IdNum, " +
                " f.transp_end_place, " +
                " f.transp_start_place, " +
                " f.driver_card_number, " +
                " f.[avto], " +
                " f.railway, " +
                " f.other, " +
                " f.pay_type, " +
                " f.pay_date, " +
                " f.comment," +
                " ISNULL(f.waybill_cost,'0')AS waybill_cost," +
                " ISNULL(f.waybill_type,'2') AS waybill_type," +
                " ISNULL(f.is_foreign,0) AS is_foreign, " +
                " f.driver_name," +
                " ISNULL(f.transport_type_id, 1)AS transport_type_id ," +
                " g.person AS contragentPerson, " +
                " g.name AS contragentName, " +
                " g.code AS contragentCode, " +
                " g.address AS contragentAddress, " +
                " g.tel AS contragentTel, " +
                " (SELECT TOP 1 address FROM book.Companies) AS companyAddress, " +
                " (SELECT TOP 1 tel FROM book.Companies) AS companyTel, " +
                " (SELECT TOP 1 name FROM book.Companies) AS companyName, " +
                " (SELECT TOP 1 code FROM book.Companies) AS companyCode, " +
                " ISNULL(NULLIF(b.waybill_num, ''), b.doc_num_prefix+CONVERT(nvarchar(20), b.doc_num)) AS doc_num, " +
                " a.product_id ," +
                " st.name AS store_name," +
                " st.code AS store_code," +
                " b.purpose," +
                " a.product_tree_path ";

                sqlQuery = sqlQuery + " FROM doc.ProductsFlow a " +
                " INNER JOIN doc.GeneralDocs b ON a.general_id = b.id " +
                " INNER JOIN (SELECT c.id,c.name AS product_nm,c.path, d.full_name AS unit_nm,c.vat, c.code AS product_code FROM book.Products c  " +
                " INNER JOIN book.Units d ON c.unit_id = d.id) e ON a.product_id = e.id " +
                " INNER JOIN doc.ProductOut f ON a.general_id = f.general_id " +
                " INNER JOIN book.Contragents g ON  b.param_id1 = g.id " +
                " INNER JOIN book.Stores st ON  b.param_id2 = st.id " +
                " WHERE a.visible=1 AND (e.path LIKE '0#1%'  OR e.path LIKE '0#3%') AND b.id = " + generalId;
                DataTable data = _nt.GetTableData(sqlQuery);
                Hashtable ar = new Hashtable();
                DateTime tdate = DateTime.Parse(data.Rows[0]["tdate"].ToString());
                ar.Add("doc_num", data.Rows[0]["doc_num"].ToString());
                ar.Add("date", tdate.ToString("dd/MM/yyyy"));
                ar.Add("time", tdate.ToString("HH:mm:ss"));
                //ar.Add("date_month", IpmReporting.MonthName.GetMonth(tdate.Month));
                //ar.Add("date_year", tdate.Year.ToString());
                ar.Add("seller_id", string.IsNullOrEmpty(data.Rows[0]["store_code"].ToString()) ? data.Rows[0]["companyCode"].ToString() : data.Rows[0]["store_code"].ToString());
                ar.Add("seller_name", string.IsNullOrEmpty(data.Rows[0]["store_code"].ToString()) ? data.Rows[0]["companyName"].ToString() : data.Rows[0]["store_name"].ToString());
                ar.Add("purchaser_id", data.Rows[0]["contragentCode"].ToString());
                ar.Add("purchaser_name", data.Rows[0]["contragentName"].ToString());
                ar.Add("payer_price", Math.Round(Convert.ToDouble(data.Rows[0]["waybill_cost"]), 2).ToString());

                string sql = @"SELECT a.id AS store_id, a.address, a.person FROM book.Stores a 
                         INNER JOIN doc.generaldocs gd on gd.param_id2=a.id WHERE gd.id=" + generalId;
                DataTable store_data = _nt.GetTableData(sql);
                string seller_person = store_data.Rows[0]["person"].ToString();
                seller_person = !string.IsNullOrEmpty(data.Rows[0]["sender_name"].ToString()) ? data.Rows[0]["sender_name"].ToString() : seller_person;


                ar.Add("seller_person", seller_person);
                ar.Add("purchaser_person", !string.IsNullOrEmpty(data.Rows[0]["reciever_name"].ToString()) ? data.Rows[0]["reciever_name"].ToString() : data.Rows[0]["contragentPerson"].ToString());
                //  ar.Add("purchaser_person", data.Rows[0]["responsable_person"].ToString());

                string address_start = store_data.Rows[0]["address"].ToString();
                if (data.Rows[0]["transp_start_place"].ToString() != "")
                    address_start = data.Rows[0]["transp_start_place"].ToString();

                string address_end = data.Rows[0]["contragentAddress"].ToString();
                if (data.Rows[0]["transp_end_place"].ToString() != "")
                    address_end = data.Rows[0]["transp_end_place"].ToString();

                ar.Add("address_start", address_start);
                ar.Add("address_end", address_end);

                ar.Add("operation_type_1", data.Rows[0]["waybill_type"].ToString() == "2" ? "ტრანსპორტირებით" : "ტრანსპორტირების გარეშე");

                string tr_type = string.Empty;
                switch (data.Rows[0]["transport_type_id"].ToString())
                {
                    case "1":
                        {
                            tr_type = "საავტომობილო";
                            if (Convert.ToInt32(data.Rows[0]["is_foreign"]) == 1)
                                tr_type = "საავტომობილო (უცხო ქვეყნის)";
                            break;
                        }

                    case "2":
                        { tr_type = "სარკინიგზო"; break; }
                    case "3":
                        { tr_type = "საავიაციო"; break; }
                    case "4":
                        { tr_type = data.Rows[0]["driver_name"].ToString(); break; }
                }
                ar.Add("transport_type", tr_type);
                ar.Add("driver_number", data.Rows[0]["transporter_IdNum"].ToString());
                ar.Add("auto_type", data.Rows[0]["transport_number"].ToString());
                ar.Add("add_info", !string.IsNullOrEmpty(data.Rows[0]["comment"].ToString()) ? data.Rows[0]["comment"].ToString() : "");
                ar.Add("print_date_time", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                //prepare products
                ArrayList datas = new ArrayList();
                ArrayList price_alls = new ArrayList();

                DataTable data_products = new DataTable();
                data_products.Columns.Add("num");
                data_products.Columns.Add("name");
                data_products.Columns.Add("code");
                data_products.Columns.Add("unit_name");
                data_products.Columns.Add("quantity");
                data_products.Columns.Add("price");
                data_products.Columns.Add("total_price");
                datas.Add(data_products);
                price_alls.Add(0.0);
                int data_cnt = 0;

                double totalPriceAll = 0.00;
                int cnt = 0;
                int total_cnt = 0;
                foreach (DataRow row in data.Rows)
                {
                    if ((cnt > 7 && datas.Count == 1) || (cnt > 29 && datas.Count > 1))
                    {
                        DataTable data_t = new DataTable();
                        data_t.Columns.Add("num");
                        data_t.Columns.Add("name");
                        data_t.Columns.Add("code");
                        data_t.Columns.Add("unit_name");
                        data_t.Columns.Add("quantity");
                        data_t.Columns.Add("price");
                        data_t.Columns.Add("total_price");
                        datas.Add(data_t);
                        price_alls.Add(0.0);
                        data_cnt++;
                        cnt = 0;
                    }
                    double price = 0.0, quantity = 0.0, total_price;

                    double.TryParse(row["price"].ToString(), out price);
                    double.TryParse(row["quantity"].ToString(), out quantity);
                    total_price = price * quantity;

                    double val = (double)price_alls[data_cnt] + total_price;
                    price_alls[data_cnt] = val;
                    ((DataTable)datas[data_cnt]).Rows.Add(total_cnt + 1,
                    row["product_name"].ToString(),
                    row["product_code"].ToString(),

                    row["unit_name"].ToString(),
                    row["quantity"].ToString(),
                    row["price"].ToString(),
                    total_price.ToString());

                    totalPriceAll += Math.Round(total_price, 2);

                    cnt++;
                    total_cnt++;
                }
                //---


                //XElement generation
                List<XElement> products = new List<XElement>();
                for(int x = 0; x < data_products.Rows.Count; x++)
                {
                    products.Add(new XElement("product",
                            new XElement("num", data_products.Rows[x]["num"]),
                            new XElement("name", data_products.Rows[x]["name"]),
                            new XElement("code", data_products.Rows[x]["code"]),
                            new XElement("unit", data_products.Rows[x]["unit_name"]),
                            new XElement("amount", data_products.Rows[x]["quantity"]),
                            new XElement("unit_price", Math.Round(Convert.ToDouble(data_products.Rows[x]["price"]), 2)),
                            new XElement("price", Math.Round(Convert.ToDouble(data_products.Rows[x]["quantity"]) * Convert.ToDouble(data_products.Rows[x]["price"]), 2))
                        ));
                }

                XElement element = new XElement("data",
                    new XElement("number", data.Rows[0]["doc_num"]),
                    new XElement("date", Convert.ToDateTime(data.Rows[0]["tdate"]).ToString("dd/MM/yyyy")),
                    new XElement("buyer_name", data.Rows[0]["contragentName"]),
                    new XElement("buyer_code", data.Rows[0]["contragentCode"]),
                    new XElement("customer_name", data.Rows[0]["companyName"]),
                    new XElement("customer_code", data.Rows[0]["companyCode"]),
                    new XElement("operation_purpose", data.Rows[0]["purpose"]),
                    new XElement("transp_start", data.Rows[0]["transp_start_place"]),
                    new XElement("transp_end", data.Rows[0]["transp_end_place"]),
                    new XElement("products", products)
                );
                //---

                return element;
            }
        }
        #endregion



        #region General Logic
        int GetCurrentUserId()
        {
            return (HttpContext.Current.Session["currentUser"] as Contragent).Id;
        }

        public string GetLanguageCookieValue()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];
            return cookie != null && !string.IsNullOrEmpty(cookie.Value) ? cookie.Value : "ka-GE";
        }

        public List<Currency> GetCurrencies()
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetList<Currency>("SELECT id, name, code FROM book.Currencies");
            }
        }

        public Currency GetContragentCurrency()
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetList<Currency>("SELECT id, name, code FROM book.Currencies WHERE code = @currencyCode", new SqlParameter { ParameterName = "@currencyCode", Value = GetCurrentContragent().Col13_555 }).FirstOrDefault();
            }
        }

        Company GetCompany()
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetList<Company>("SELECT id, name, code, tel, address, info, chief FROM book.Companies").FirstOrDefault();
            }
        }

        public Staff GetContragentStaff()
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetList<Staff>("SELECT id, name FROM book.Staff WHERE name LIKE N'%" + GetCurrentContragent().Col12_554 + "%'").FirstOrDefault();
            }
        }

        int? GetUserIdByStaff()
        {
            using (NTContext _nt = new NTContext())
            {
                return _nt.GetScalar<int>("SELECT id FROM book.Users WHERE staff_id = @staffId", new SqlParameter { ParameterName = "@staffId", Value = GetContragentStaff().Id });
            }
        }
        #endregion



        #region FINA Operations
        bool OrderFromBuyer(List<ShoppingCart> shoppingCarts, int contragentAddressId) //შეკვეთა მყიდველისგან
        {
            var request = (HttpWebRequest)WebRequest.Create("http://rates.fina.ge:8081/CurrencyService/CurrencyService.svc/GetRateByDate/currency=" + GetContragentCurrency().Code + "/date=" + DateTime.Now.ToString("yyyy-MM-dd"));
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var rateResult = JsonConvert.DeserializeObject<dynamic>(responseString);
            double currRate = rateResult.rate;

            using (NTContext _nt = new NTContext())
            {
                long? docNum = _nt.GetScalar<long>("SELECT MAX(gd.doc_num) FROM doc.GeneralDocs AS gd WHERE gd.doc_type = @docType", new SqlParameter { ParameterName = "@docType", Value = 8 }); //შეკვეთა მყიდველისგან

                int? generalId = _nt.InsertGeneralDoc
                     (
                         DateTime.Now,
                         string.Empty,
                         docNum.HasValue ? docNum.Value + 1 : 0,
                         8, //შეკვეთა მყიდველისგან
                         string.Format("შეკვეთა B2B -დან № {0}", docNum.HasValue ? docNum.Value + 1 : 0),
                         shoppingCarts.Select(shoppingCart => (double)shoppingCart.Price * shoppingCart.Quantity).Sum(),
                         GetContragentCurrency().Id,
                         currRate,
                         18,
                         GetUserIdByStaff(), //user_id
                         0,
                         GetCurrentUserId(),
                         _storeId,
                         1,
                         false,
                         1,
                         1,
                         GetContragentStaff().Id
                     );

                if (generalId.HasValue)
                {
                    var result = false;

                    foreach(var shoppingCart in shoppingCarts)
                    {
                        result = _nt.InsertProductsFlow
                        (
                            shoppingCart.ProductId,
                            "",
                            generalId.Value,
                            shoppingCart.Quantity,
                            (double)shoppingCart.Price,
                            _storeId,
                            18,
                            (double)shoppingCart.Price / 1.18,
                            -1,
                            1,
                            0,
                            0,
                            1,
                            0,
                            1,
                            "",
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            "",
                            0
                        );

                        if (!result)
                        {
                            return false;
                        }
                    }

                    Contragent currentContragent = GetCurrentContragent();
                    string payType = "1"; //დეფაულტად უნაღდო (თუ არაფერია მითითებული)
                    if(currentContragent.Col3_545 == "1")
                    {
                        payType = "2"; //კონსიგნაცია
                    }
                    if(currentContragent.Col4_546 == "1")
                    {
                        payType = "3"; //განვადება
                    }
                    if (currentContragent.Col5_547 == "1")
                    {
                        payType = "6"; //სელაუტი (სხვა)
                    }
                    if (currentContragent.Col6_548 == "1")
                    {
                        payType = "1"; //უნაღდო
                    }


                    ContragentAddress transpEndAddress = GetContragentAddresses().Where(ca => ca.Id == contragentAddressId).FirstOrDefault();

                    result = _nt.InsertCustomerOrder
                    (
                        generalId.Value,
                        0,
                        1,
                        6,
                        "3",
                        "",
                        false,
                        0,
                        payType,
                        DateTime.Now,
                        0,
                        GetContragentStaff().Id,
                        DateTime.Now,
                        DateTime.Now,
                        0,
                        string.Empty,
                        transpEndAddress == null ? GetCurrentContragent().Address : transpEndAddress.Address,
                        GetContragentSpecialPrice().Id
                    );

                    return result;
                }
                else
                {
                    return false;
                }
            }
        }        
        #endregion
    }
}