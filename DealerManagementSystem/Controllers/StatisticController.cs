using DealerManagementSystem.Models;
using DealerManagementSystem.Utils;
using System;
using System.Web.Mvc;
using System.Linq;
using System.Data.SqlClient;

namespace DealerManagementSystem.Controllers
{
    [ValidateUserFilter]
    public class StatisticController : BaseController
    {
        BusinessLogic _bl = new BusinessLogic();

        public JsonResult CategoryCycleChange(int year, int contragent_id)
        {
            using (NTContext _db = new NTContext())
            {
                return Json(_db.GetTableDictionary(@"DECLARE @year AS INT = " + year + @"
DECLARE @contr_id AS INT = " + contragent_id + @"

; WITH
 realiz([month], group_name, amount)AS
 (
   SELECT DATEPART(month, g.tdate) AS[month], p.usr_column_563, ROUND(SUM(fl.amount * fl.price * (-fl.coeff)),4) AS amount
 
   FROM doc.generaldocs AS g
 
   INNER JOIN doc.productsflow AS fl ON fl.general_id = g.id
 
   INNER JOIN book.Products AS p ON p.id = fl.product_id
 
   WHERE g.doc_type = 21 AND DATEPART(year, g.tdate) = @year AND g.param_id1 = @contr_id  AND(g.is_deleted IS NULL OR g.is_deleted = 0)  AND(fl.ref_uid IS NULL  OR fl.ref_uid = '') AND p.usr_column_563 IS NOT NULL
 
   GROUP BY p.usr_column_563, DATEPART(month, g.tdate)
 )

SELECT group_name, ISNULL([1], 0) AS[One],ISNULL([2], 0) AS[Two],ISNULL([3], 0) AS[Three],ISNULL([4], 0) AS[Four],ISNULL([5], 0) AS[Five],ISNULL([6], 0) AS[Six],ISNULL([7], 0) AS[Seven],ISNULL([8], 0) AS[Eight],ISNULL([9], 0) AS[Nine],ISNULL([10], 0) AS[Then],ISNULL([11], 0) AS[Eleven],ISNULL([12], 0) AS[Twelve]
FROM realiz
PIVOT(MIN(amount) FOR[month] IN([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])) AS PVTTable
OPTION (MAXRECURSION 0)"));
            }
        }

        public JsonResult CategorySummaryChange(int year, int contragent_id)
        {
            using (NTContext _db = new NTContext())
            {
                DateTime date_from = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local);
                return Json(_db.GetTableDictionary(@"
DECLARE @contr_id AS INT = " + contragent_id + @"

SELECT  p.usr_column_563 as name, ROUND(SUM(fl.amount * fl.price*g.rate*(-fl.coeff)),4) AS y
FROM doc.generaldocs AS g
INNER JOIN doc.ProductsFlow AS fl ON fl.general_id=g.id
INNER JOIN book.Products AS p ON p.id=fl.product_id
WHERE g.doc_type IN(21,9) AND g.param_id1=@contr_id AND g.tdate BETWEEN @date1 AND @date2 AND fl.visible=1  AND p.usr_column_563 IS NOT NULL
GROUP BY p.usr_column_563", new System.Data.SqlClient.SqlParameter("@date1", date_from), new System.Data.SqlClient.SqlParameter("@date2", date_from.AddYears(1))));
            }
        }

        public JsonResult InReturnChange(int year, int contragent_id)
        {
            using (NTContext _db = new NTContext())
            {
                return Json(_db.GetTableDictionary(@"DECLARE @year AS INT = " + year + @"
DECLARE @contr_id AS INT = " + contragent_id + @"

;WITH realiz([month], oper_type, amount)AS
(
  SELECT DATEPART(month, g.tdate) AS [month], tp.name,  ROUND(SUM(fl.amount*fl.price*(-fl.coeff)),4) AS amount
  FROM doc.generaldocs AS g
  INNER JOIN doc.productsflow AS fl ON fl.general_id=g.id
  INNER JOIN book.Products AS p ON p.id=fl.product_id
  INNER JOIN config.doctypes AS tp ON tp.id=g.doc_type
  WHERE g.doc_type IN(9,21) AND g.param_id1=@contr_id AND DATEPART(year, g.tdate)=@year  AND (g.is_deleted IS NULL OR g.is_deleted=0)  AND (fl.ref_uid IS NULL  OR fl.ref_uid='') AND (p.path='0#1#10' OR p.path LIKE '0#1#10#%' )
  GROUP BY tp.name,DATEPART(month, g.tdate)
)

SELECT oper_type ,ISNULL([1], 0) AS [One],ISNULL([2], 0) AS [Two],ISNULL([3], 0) AS [Three],ISNULL([4], 0) AS [Four],ISNULL([5], 0) AS [Five],ISNULL([6], 0) AS [Six],ISNULL([7], 0) AS [Seven],ISNULL([8], 0) AS [Eight],ISNULL([9], 0) AS [Nine],ISNULL([10], 0) AS [Then],ISNULL([11], 0) AS [Eleven],ISNULL([12], 0) AS [Twelve]  
FROM realiz
PIVOT(MIN(amount) FOR [month] IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])) AS PVTTable
WHERE oper_type IS NOT NULL
OPTION (MAXRECURSION 0)"));
            }
        }


        public JsonResult DebtChange(int contragent_id)
        {
            using (NTContext _db = new NTContext())
            {               
                var result = _db.GetList<DebtChangeModel>(@"DECLARE @date AS DATETIME = getdate()
                                                            DECLARE @curr AS INT = " + _bl.GetContragentCurrency().Id + @"
                                                            DECLARE @contr_id AS INT = " + contragent_id + @"

                                                            DECLARE @tdate AS VARCHAR(30) = (select top 1 db_name from config.columns where table_id='TABLE_CONTRAGENT' AND full_name=N'შეთანხმების თარიღი')
                                                            DECLARE @tdate_term AS VARCHAR(30) = (select top 1 db_name from config.columns where table_id='TABLE_CONTRAGENT' AND full_name=N'შეთანშმების მოქმედების ვადა')

                                                            EXEC('
                                                            ;WITH   contragents(id, account, account2) AS(SELECT id, account, account2 FROM book.contragents WHERE id='+@contr_id+'),
		                                                            debt(id, debet, credit)AS
		                                                            (
			                                                            SELECT r.id, r.debit, r.credit 
			                                                            FROM (
			                                                            SELECT c.id, 
			                                                            CAST((SELECT ISNULL(SUM(e.amount * CASE '+@curr+' WHEN 0 THEN e.rate ELSE 1 END ),0) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id WHERE (0='+@curr+' OR '+@curr+'=e.currency_id) AND  e.a1=c.id AND (e.debit_acc =  c.account OR e.debit_acc =  c.account2)) AS DEC(38,2)) AS debit,
			                                                            CAST((SELECT ISNULL(SUM(e.amount * CASE '+@curr+' WHEN 0 THEN e.rate ELSE 1 END ),0) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id WHERE (0='+@curr+' OR '+@curr+'=e.currency_id) AND  e.b1=c.id AND (e.credit_acc =  c.account OR e.credit_acc =  c.account2))AS DEC(38,2)) AS credit
			                                                            FROM contragents AS c)
			                                                            AS r
		                                                            ),
		                                                            flow(num, id, general_id, due_date, debit, dafaruli) AS
		                                                            (
		                                                            SELECT ROW_NUMBER()OVER(ORDER BY r.id, r.tdate) AS num, r.id, r.general_id, r.tdate, CAST(ISNULL(r.deb,0) AS DEC(38,5)) AS deb, r.dafaruli 
		                                                            FROM(
			                                                            select c.id, g.id AS general_id, pd.id AS deadliene_id,  COALESCE(pd.tdate, g.tdate) AS tdate, COALESCE(pd.amount * CASE '+@curr+' WHEN 0 THEN g.rate ELSE 1 END, SUM(e.amount*CASE '+@curr+' WHEN 0 THEN e.rate ELSE 1 END))AS deb, d.credit AS dafaruli
			                                                            FROM debt AS d
			                                                            INNER JOIN contragents AS c ON c.id=d.id
			                                                            INNER JOIN doc.entries AS e ON e.a1=c.id AND (e.debit_acc =  c.account OR e.debit_acc =  c.account2)
			                                                            INNER JOIN doc.GeneralDocs AS g ON g.id=e.general_id
			                                                            LEFT JOIN doc.PaymentDeadlines AS pd ON pd.general_id=g.id
		                                                                  WHERE (0='+@curr+' OR '+@curr+' = e.currency_id )
			                                                            GROUP BY  c.id,  g.id, pd.id, COALESCE(pd.tdate,g.tdate),  pd.amount*CASE '+@curr+' WHEN 0 THEN g.rate ELSE 1 END, d.credit
			                                                            ) AS r
		                                                            ),
		                                                            flow2(num, id, general_id, due_date, overdue_days, debit, credit, rest) AS 
		                                                            (
		                                                            SELECT ROW_NUMBER()OVER(ORDER BY r.num) AS num, r.id, r.general_id, r.due_date,  CASE WHEN r.dafaruli-r.runing < 0 THEN DATEDIFF(dd, '''+@date+''',r.due_date ) ELSE 0 END AS overdue, r.debit, CASE WHEN r.dafaruli- r.runing > 0 THEN r.debit ELSE CASE WHEN r.debit - ABS(r.dafaruli-r.runing) < 0 THEN 0 ELSE r.debit - ABS(r.dafaruli-r.runing)END END AS credit, r.dafaruli-r.runing
		                                                            FROM(
		                                                            SELECT  num, id,  general_id, due_date, debit, dafaruli, SUM(debit) OVER(PARTITION BY id ORDER BY num) AS runing  
		                                                            FROM flow 
		                                                            ) AS r
		                                                            ),
		                                                            flow3(num, id, general_id, due_date, overdue_days, debit, credit, rest) AS
		                                                            (
		                                                            SELECT 	num, id, general_id, due_date, overdue_days AS overdue_days, debit, credit,  debit-credit AS rest
		                                                            FROM flow2 WHERE flow2.rest  < 0 AND flow2.overdue_days < 0
                                                                    ),
                                                                    flow4( id, due_date, overdue_days, rest)AS
                                                                    (
                                                                    SELECT  id, min(due_date), min(overdue_days), SUM(rest)
                                                                    FROM flow3
                                                                    GROUP BY id
                                                                    ),
		                                                            terms(val) AS
		                                                            (
			                                                            SELECT  DATEDIFF(dd, GETDATE(),  DATEADD(dd, CONVERT(INT,c.'+@tdate_term+'), CONVERT(DATE, c.'+@tdate+')) )
			                                                            FROM book.contragents AS c
			                                                            WHERE c.id='+@contr_id+' AND ISDATE(c.'+@tdate+')=1 AND ISNUMERIC(c.'+@tdate_term+')=1
		                                                            )
		
		                                                            SELECT ISNULL((SELECT CAST(val AS VARCHAR(200)) FROM terms),'''') AS remaining_time,
			                                                               ISNULL((SELECT ROUND(rest,2) FROM flow4),0) AS mimdinare_gadasaxdeli,
			                                                               ISNULL((SELECT ROUND(debet-credit,2) FROM debt),0) AS sruli_mimdinare_davalianeba,
			                                                               ISNULL((SELECT ROUND(AVG(rest),2) FROM flow3),0) AS avg_overdue_amount,
			                                                               ISNULL((SELECT ROUND(AVG(ABS(overdue_days)),2) FROM flow3),0) AS avg_overdue_days
                                                            ')").FirstOrDefault();

                var contragent = _db.GetList<Contragent>("SELECT usr_column_555 AS Col13_555 FROM book.Contragents WHERE id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = contragent_id }).FirstOrDefault();

                result.contragentCurrency = contragent.Col13_555;

                return Json(result);
            }
        }


        public JsonResult GenerateQueries(string dateFrom, string dateTo)
        {
            using (NTContext _db = new NTContext())
            {
                int contragentId = _bl.GetCurrentContragent().Id;

                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@dateFrom", System.Data.SqlDbType.DateTime) { Value = dateFrom },
                    new SqlParameter("@dateTo", System.Data.SqlDbType.DateTime) { Value = dateTo },
                    new SqlParameter("@contragentId", System.Data.SqlDbType.Int) { Value = contragentId },
                    new SqlParameter("@currencyId", System.Data.SqlDbType.Int) { Value = _bl.GetContragentCurrency().Id }
                };

                var result = _db.GetList<GeneratedQueryResult>(@"DECLARE @date1 AS DATETIME = @dateFrom
                                                                DECLARE @date2 AS DATETIME = @dateTo
                                                                DECLARE @contr_id AS INT = @contragentId
                                                                DECLARE @curr AS INT = @currencyId

                                                                ;WITH cte1(variet)AS
	                                                                (
		                                                                SELECT
		                                                                CAST((SELECT ISNULL(SUM(e.amount * CASE @curr WHEN 0 THEN e.rate ELSE 1 END),0) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id WHERE (0=@curr OR @curr=e.currency_id) AND g.tdate<@date2 AND  e.a1=c.id AND (e.debit_acc =  c.account OR e.debit_acc =  c.account2)) AS DEC(38,2)) -
		                                                                CAST((SELECT ISNULL(SUM(e.amount * CASE @curr WHEN 0 THEN e.rate ELSE 1 END),0) FROM doc.Entries AS e INNER JOIN doc.GeneralDocs g ON e.general_id = g.id WHERE (0=@curr OR @curr=e.currency_id) AND g.tdate<@date2 AND e.b1=c.id AND (e.credit_acc =  c.account OR e.credit_acc =  c.account2))AS DEC(38,2)) 
		                                                                FROM book.Contragents AS c 
		                                                                WHERE c.id=@contr_id
		                                                                    ),
	                                                                cte2(invoie_count, avg_invoice_amount)AS
	                                                                (
		                                                                SELECT COUNT(id) AS invoie_count, ISNULL(ROUND(AVG(amount * CASE @curr WHEN 0 THEN rate ELSE 1 END),2),0) 
	                                                                    FROM doc.GeneralDocs 
		                                                                WHERE tdate BETWEEN @date1 AND @date2 AND doc_type=8 AND param_id1=@contr_id AND (0=@curr OR @curr = currency_id)
	                                                                ),

	                                                                cte3(ret_val)AS
	                                                                (
		                                                                SELECT ISNULL(ROUND(SUM(amount * CASE @curr WHEN 0 THEN rate ELSE 1 END),2),0) 
	                                                                    FROM doc.GeneralDocs 
		                                                                WHERE tdate BETWEEN @date1 AND @date2 AND doc_type=9 AND param_id1=@contr_id AND (0=@curr OR @curr = currency_id)
	                                                                )

	                                                                SELECT
	                                                                (SELECT invoie_count FROM cte2) AS invoie_count,
	                                                                (SELECT avg_invoice_amount FROM cte2) AS avg_invoice_amount,
	                                                                ISNULL((SELECT variet FROM cte1),0) AS full_variet,
	                                                                (SELECT ret_val FROM cte3) AS ret_val, 
	                                                                ISNULL((SELECT variet FROM cte1),0)-(SELECT ret_val FROM cte3) AS real_variet,
	                                                                ISNULL(ROUND(((SELECT ret_val FROM cte3)/(SELECT NULLIF(variet,0) FROM cte1)*100),2),0) AS perc", sqlParams).FirstOrDefault();

                var contragent = _db.GetList<Contragent>("SELECT usr_column_555 AS Col13_555 FROM book.Contragents WHERE id = @contragentId", new SqlParameter { ParameterName = "@contragentId", Value = contragentId }).FirstOrDefault();

                result.contragentCurrency = contragent.Col13_555;

                return Json(result);
            }
        }

    }
}