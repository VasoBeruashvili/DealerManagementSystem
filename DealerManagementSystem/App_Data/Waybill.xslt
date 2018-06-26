<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output
          media-type="text/html" method="html" encoding="utf-8"
          doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"
    />

  <xsl:template match="/">
    <html>
      <head>
       <meta content="text/html; charset=utf-8" http-equiv="content-type" />
        <style type="text/css">

          *{margin: 0;padding: 0;outline:0;line-height:1.1em;}
          body{font-size: 12px;}
          .body_div{width:768px;height:1024px; margin-top:15px;}
          .base{border: 1px solid black;height: 19px;margin-top: 15px;}
          .base div{float: left;padding-left: 7px;border-left: 1px solid black;height: 17px;padding-top: 2px;}
          .base div:first-child{border: none;width:15px;}
          .f_small {font-size: 8px; vertical-align: top;}
          .rotate-90{display: block;margin-top: 15px;-webkit-transform: rotate(-90deg);text-align: center;width: 58px;margin-left:-19px;}
          .grid{margin-left: 15px; background-color: black;}
          .grid tr:first-child td { height: 52px;text-align: center;}
          .grid tr td { font-size: 11px;background-color: white; text-align: center; height: 22px;}
          .td_left{text-align:left !important; padding-left:3px;}
        </style>
      </head>
      <body>
        <div class="body_div">
          <div style="margin-left: 154px;">
            <div style="width: 450px;" class="base">
              <div>1</div>
              <div style="width: 60%;">სასაქონლო ზედნადები №</div>
              <div style="width: 25%;">
                ელ-<xsl:value-of select="*/number" />
              </div>
            </div>
          </div>
          <div>
            <div style="width: 220px; margin-left: 58px; display: inline-block;" class="base">
              <div>2</div>
              <div>
                <xsl:value-of select="*/date" />
              </div>
            </div>
            <div style="width: 220px; margin-right: 80px; float: right;" class="base">
              <div>3</div>
              <div>
                <xsl:value-of select="*/time" />
              </div>
            </div>
            <div>
              <span style="margin-left: 110px;" class="f_small">თარიღი(რიცხვი,თვე,წელი)</span>
              <span style="margin-right: 140px; float: right;" class="f_small">დრო (საათი,წუთი)</span>
            </div>
          </div>
          <div>
            <div style="width: 360px; margin-left: 15px; display: inline-block;" class="base">
              <div>4</div>
              <div style="width: 70%;">
                <xsl:value-of select="*/buyer_name" />
                <xsl:text> </xsl:text>
              </div>
              <div style="width: 10%;">
                <xsl:value-of select="*/buyer_code" />
                <xsl:text> </xsl:text>
              </div>
            </div>
            <div style="width: 360px; margin-right: 15px; float: right;" class="base">
              <div>5</div>
              <div style="width: 70%;">
                <xsl:value-of select="*/customer_name" />
              </div>
              <div style="width: 10%;">
                <xsl:value-of select="*/customer_code" />
              </div>
            </div>
            <div>
              <div style="display: inline-block;">
                <span style="padding-left:43px; display: block;" class="f_small">გამყიდველის (გამგზავნის) დასახელება,ან სახელი  და</span>
                <span style="padding-left:130px; display: block;" class="f_small">გვარი</span>
              </div>
              <div style="display: inline-block; vertical-align: top;margin-left:2px;">
                <span style="display: block; padding-left:28px;" class="f_small">საიდენტიფიკაციო/პირადი</span>
                <span style="display: block; padding-left:60px;" class="f_small">ნომერი</span>
              </div>
              <div style="display: inline-block;margin-left:13px;">
                <span style="padding-left:55px; display: block;" class="f_small">მყიდველი (მიმღების) დასახელება,ან სახელი და</span>
                <span style="padding-left:115px; display: block;" class="f_small">გვარი</span>
              </div>
              <div style="display: inline-block; vertical-align: top;">
                <span style="display: block; padding-left:30px;" class="f_small">საიდენტიფიკაციო/პირადი</span>
                <span style="display: block; padding-left:70px;" class="f_small">ნომერი</span>
              </div>
            </div>
          </div>
          <div>
            <div style="width: 315px; margin-left: 15px; display: inline-block; height: 50px;" class="base">
              <div style="height: 48px;">6</div>
              <div style="height: 48px;width: 10%; font-size: 9px;" >
                <span class="rotate-90">ოპერაციის შინაარსი</span>
              </div>
              <div style="height: 48px;width: 20%;">
                <xsl:value-of select="*/operation_purpose" />
              </div>
            </div>
            <div style="float: right;">
              <div style="width: 380px; margin-right: 15px;" class="base">
                <div>7</div>
                <div>
                  <xsl:value-of select="*/transp_start" />
                  <xsl:text> </xsl:text>
                </div>
              </div>
              <div>
                <span style="padding-left:110px;" class="f_small">ტრანსპორტირების დაწყების ადგილი (მისამართი)</span>
              </div>
              <div style="width: 380px; margin-right: 15px; margin-top:1px;" class="base">
                <div>8</div>
                <div>
                  <xsl:value-of select="*/transp_end" />
                  <xsl:text> </xsl:text>
                </div>
              </div>
              <div>
                <span style="padding-left:110px;" class="f_small">ტრანსპორტირების დასრულების ადგილი (მისამართი)</span>
              </div>
            </div>
            <div style="display: inline-block;">
              <span style="padding-left:110px;" class="f_small">მიწოდებია სახე</span>
            </div>
          </div>
          <div>
            <div style="width: 260px; margin-left: 15px; display: inline-block;" class="base">
              <div>9</div>
              <div>
                <xsl:value-of select="*/transport_type" />
              </div>
            </div>
            <div style="width:280px; margin-left: 15px; display: inline-block;" class="base">
              <div>10</div>
              <div>
                <xsl:value-of select="*/transport_number" />
              </div>
            </div>
            <div class="base" style="width: 155px;display: inline-block;margin-left: 16px;">
              <div>X</div>
            </div>
          </div>
          <div>
            <span style="padding-left:100px; display: inline-block" class="f_small">ტრანსპორტირების სახე</span>
            <span style="padding-left:160px; display: inline-block" class="f_small">ტრანსპორტირების საშუალების სახელმწიფო ნომერი</span>
            <span style="padding-left:110px; display: inline-block" class="f_small">მისაბმელი</span>
          </div>
          <div>
            <div style="width: 320px; margin-left: 15px; display: inline-block; height: 30px;" class="base">
              <div style=" height: 29px;">11</div>
              <div style="width: 53%; font-size: 10px; text-align:center; height: 29px;">ტრანსპორტირების საშუალების მძღოლის პირადი ნომერი</div>
              <div style="width: 20%; height: 29px;">
                <xsl:value-of select="*/driver_code" />
                <xsl:text> </xsl:text>
              </div>
            </div>
            <div style="width: 400px; margin-right: 15px; float: right; height: 30px;" class="base">
              <div style=" height: 29px;">12</div>
              <div style="width: 72%; font-size: 10px; height: 29px; text-align:center;">გამყიდველის(გამგზავნის)/მყიდველის(მიმღების) მიერ გაწეულიტრანსპორტირების ხარჯი</div>
              <div style="width: 18%; height: 29px; text-align:right;">
                <xsl:value-of select="*/transport_xarji" />
                <xsl:text> </xsl:text>
              </div>
            </div>
            <div style="text-align: right;">
              <span style="padding-right:25px; display: block;" class="f_small">თანხა ლარებში</span>
            </div>
          </div>
          <div style="margin: 40px 0 12px 0px; font-size: 18px; text-align: center;">
                                     სასაქონლო ზედნადების ცხრილი
          </div>
          <div style=" width:753px;height:362px;">
            <table cellpadding="1" cellspacing="1" border="0" class="grid">
              <tr>
                <td width="3%">
                  <span class="rotate-90" style="margin:0 -12px 0 -17px;">რიგითი N</span>
                </td>
                <td width="40%">საქონლის დასახელება</td>
                <td width="12%">საქონლის კოდი</td>
                <td width="15%">საქონლის ზომის ერთეული</td>
                <td width="10%">საქონლის რაოდენობა</td>
                <td width="10%">საქონლის ერთეულის ფასი*</td>
                <td width="10%">საქონლის ფასი*</td>
              </tr>
              <tr>
                <td>I</td>
                <td>II</td>
                <td>III</td>
                <td>IV</td>
                <td>V</td>
                <td>VI</td>
                <td>VII</td>
              </tr>
              <xsl:for-each select="*/products/product">
                <tr>
                  <td>
                    <xsl:value-of select="num" />
                  </td>
                  <td class="td_left">
                    <xsl:value-of select="name" />
                  </td>
                  <td class="td_left">
                    <xsl:value-of select="code" />
                  </td>
                  <td class="td_left">
                    <xsl:value-of select="unit" />
                  </td>
                  <td>
                    <xsl:value-of select="amount" />
                  </td>
                  <td>
                    <xsl:value-of select="unit_price" />
                  </td>
                  <td>
                    <xsl:value-of select="price" />
                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </div>
          <div>
            <div style="float: right;">
              <div style="width: 397px; margin-right: 15px;" class="base">
                <div>13</div>
                <div>
                  <xsl:value-of select="*/full_amount" />
                </div>
              </div>
              <div>
                <span style="padding-left: 40px;" class="f_small">მიწოდებული საქონლის მთლიანი თანხა (ციფრებით და სიტყვებით)</span>
              </div>
            </div>
          </div>
          <div>
            <div style="width: 350px; margin-left: 15px; display: inline-block;" class="base">
              <div>14</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
            </div>
            <div style="width: 353px; margin-right: 14px; float: right;" class="base">
              <div>15</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
            </div>
            <div style="display: inline-block; width: 355px; text-align: center;">
              <span style="padding-left:48px; display: block;" class="f_small">გამყიდველი (გამგზავნი) - საქონლის ჩაბარებაზე უფლებამოსილი პირი (თანამდებობა, სახელი, გვარი)</span>
            </div>
            <div style="display: inline-block; float: right; width: 374px; text-align: center;">
              <span style=" margin-left: 20px;" class="f_small">მყიდველი (მიმღები) - საქონლის ჩაბარებაზე უფლებამოსილი პირი (თანამდებობა,</span>
              <span style=" margin-left: 22px;" class="f_small">სახელი, გვარი)</span>
            </div>
          </div>

          <div>
            <table style="width:100%;">
              <tr>
                <td>
                   <div style="width: 250px; margin-left: 15px; display: inline-block;" class="base">
              <div>16</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
                 </div>
              </td>    
               <td>
               <div style="width: 255px;margin-left: 215px;  float: left;" class="base">
              <div>17</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
            </div>
                </td>
              </tr>
            </table> 
            <div style="display: inline-block; width: 335px; text-align: center;">
              <span class="f_small" style="margin-right:54px;">ხელმოწერა</span>
            </div>
            <div style="float: right; width: 273px; text-align: center;">
              <span class="f_small">ხელმოწერა</span>
            </div>
          </div>
          <div>
            <div style="width: 500px; margin-right: 18px;float: right; height: 26px;" class="base">
              <div style=" height: 25px;">18</div>
              <div style="width: 30%;font-size: 10px; text-align: center; height: 25px;">მიწოდებული საქონელის ჩაბარების</div>
              <div style="width: 35%; height: 25px;">
                <xsl:value-of select="*/prod_date" />
              </div>
              <div style="width: 25%; height: 25px;">
                <xsl:value-of select="*/prod_time" />
              </div>
            </div>
            <div>
              <span style="padding-left: 445px;" class="f_small">თარიღი(რიცხვი, თვე, წელი)</span>
              <span style="padding-left: 33px;" class="f_small">დრო (საათი,წუთი)</span>
            </div>
          </div>
          <div>
            <div style="margin-left: 15px; width: 738px;" class="base">
              <div>19</div>
              <div><xsl:text> </xsl:text></div>
            </div>
            <div>
              <span style="text-align: center; display: block;" class="f_small">შენიშვნა</span>
            </div>
          </div>
          <div>
            <div style="width: 738px; margin: 14px 0 0 15px;">
              <span class="f_small">* დღგ-ს გადამხდელისათვის დღგ-ს ჩათვლით, აქციზის გადამხდელისათვის აქციზურ საქონელზე, დღგ-ს და აქციზის ჩათვლით</span>
            </div>
          </div>
          <div style="margin-top:10px;">
          <span style="margin-left:45px; font-size:13px;">ამობეჭვდის თარიღი: </span>
          <span><xsl:value-of select="*/print_date" /></span>
          </div>
        </div>
        
        
        <xsl:for-each select="*/products/page[position() &gt; 1]">
          <div class="body_div">
          <div style="margin: 42px 0 7px 0px; font-size: 17px; text-align: center;">
            <xsl:value-of select="/*/number" />    სასაქონლო ზედნადების დანართი
          </div>
            <div style="width:753px;height:736px;">
              <table cellpadding="1" cellspacing="1" border="0" class="grid">
                <tr>
                  <td width="3%">
                    <span class="rotate-90" style="margin:0 -12px 0 -17px;">რიგითი N</span>
                  </td>
                  <td width="40%">საქონლის დასახელება</td>
                  <td width="12%">საქონლის კოდი</td>
                  <td width="15%">საქონლის ზომის ერთეული</td>
                  <td width="10%">საქონლის რაოდენობა</td>
                  <td width="10%">საქონლის ერთეულის ფასი*</td>
                  <td width="10%">საქონლის ფასი*</td>
                </tr>
                <tr>
                  <td>I</td>
                  <td>II</td>
                  <td>III</td>
                  <td>IV</td>
                  <td>V</td>
                  <td>VI</td>
                  <td>VII</td>
                </tr>
                <xsl:for-each select="product">
                  <tr>
                    <td>
                      <xsl:value-of select="num" />
                    </td>
                    <td class="td_left">
                      <xsl:value-of select="name" />
                    </td>
                    <td class="td_left">
                      <xsl:value-of select="code" />
                    </td>
                    <td class="td_left">
                      <xsl:value-of select="unit" />
                    </td>
                    <td>
                      <xsl:value-of select="amount" />
                    </td>
                    <td>
                      <xsl:value-of select="unit_price" />
                    </td>
                    <td>
                      <xsl:value-of select="price" />
                    </td>
                  </tr>
                </xsl:for-each>
              </table>
            </div>
            <div>
            <div style="float: right;">
              <div style="width: 397px; margin-right: 15px;" class="base">
                <div>13</div>
                <div>
                  <xsl:value-of select="/*/full_amount" />
                </div>
              </div>
              <div>
                <span style="padding-left: 40px;" class="f_small">მიწოდებული საქონლის მთლიანი თანხა (ციფრებით და სიტყვებით)</span>
              </div>
            </div>
          </div>
          <div>
            <div style="width: 350px; margin-left: 15px; display: inline-block;" class="base">
              <div>14</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
            </div>
            <div style="width: 353px; margin-right: 14px; float: right;" class="base">
              <div>15</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
            </div>
            <div style="display: inline-block; width: 355px; text-align: center;">
              <span style="padding-left:48px; display: block;" class="f_small">გამყიდველი (გამგზავნი) - საქონლის ჩაბარებაზე უფლებამოსილი პირი (თანამდებობა, სახელი, გვარი)</span>
            </div>
            <div style="display: inline-block; float: right; width: 374px; text-align: center;">
              <span style=" margin-left: 30px;" class="f_small">მყიდველი (მიმღები) - საქონლის ჩაბარებაზე უფლებამოსილი პირი (თანამდებობა,</span>
              <span style=" margin-left: 22px;" class="f_small">სახელი, გვარი)</span>
            </div>
          </div>

          <div>
            <table style="width:100%;">
              <tr>
                <td>
                   <div style="width: 250px; margin-left: 15px; display: inline-block;" class="base">
              <div>16</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
                 </div>
              </td>    
               <td>
               <div style="width: 255px;margin-left: 215px;  float: left;" class="base">
              <div>17</div>
              <div>
                <xsl:text> </xsl:text>
              </div>
            </div>
                </td>
              </tr>
            </table> 
            <div style="display: inline-block; width: 335px; text-align: center;">
              <span class="f_small" style="margin-right:54px;">ხელმოწერა</span>
            </div>
            <div style="float: right; width: 273px; text-align: center;">
              <span class="f_small">ხელმოწერა</span>
            </div>
          </div>
          <div>
            <div style="width: 500px; margin-right: 18px;float: right; height: 26px;" class="base">
              <div style=" height: 25px;">18</div>
              <div style="width: 30%;font-size: 10px; text-align: center; height: 25px;">მიწოდებული საქონელის ჩაბარების</div>
              <div style="width: 35%; height: 25px;">
                <xsl:value-of select="/*/prod_date" />
              </div>
              <div style="width: 25%; height: 25px;">
                <xsl:value-of select="/*/prod_time" />
              </div>
            </div>
            <div>
              <span style="padding-left: 445px;" class="f_small">თარიღი(რიცხვი, თვე, წელი)</span>
              <span style="padding-left: 33px;" class="f_small">დრო (საათი,წუთი)</span>
            </div>
          </div>
          <div>
            <div style="margin-left: 15px; width: 738px;" class="base">
              <div>19</div>
              <div><xsl:text> </xsl:text></div>
            </div>
            <div>
              <span style="text-align: center; display: block;" class="f_small">შენიშვნა</span>
            </div>
          </div>
          <div>
            <div style="width: 738px; margin: 14px 0 0 15px;">
              <span class="f_small">* დღგ-ს გადამხდელისათვის დღგ-ს ჩათვლით, აქციზის გადამხდელისათვის აქციზურ საქონელზე, დღგ-ს და აქციზის ჩათვლით</span>
            </div>
          </div>
          <div style="margin-top:10px;">
          <span style="margin-left:45px; font-size:13px;">ამობეჭვდის თარიღი: </span>
          <span><xsl:value-of select="/*/print_date" /></span>
          </div>
        </div>
        </xsl:for-each>
      </body>
    </html>

  </xsl:template>
</xsl:stylesheet>