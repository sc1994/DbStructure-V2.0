using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using API.Common;
using API.Models;

namespace API.Controllers
{
    public class TableController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetTableInfo(string table, string dbName)
        {
            var tableName = table.Split('(')[0];

            var tableInfo = DbClient.Query<TableInfo>($@"USE {dbName};
                                                     SELECT a.name AS fieldname ,
                                                            ( CASE WHEN COLUMNPROPERTY(a.id, a.name, 'IsIdentity') = 1
                                                                   THEN '√'
                                                                   ELSE ''
                                                              END ) AS identifying ,
                                                            ( CASE WHEN ( SELECT    COUNT(*)
                                                                          FROM      sysobjects--查询主键  
                                                                          WHERE     ( name IN (
                                                                                      SELECT    name
                                                                                      FROM      sysindexes
                                                                                      WHERE     ( id = a.id )
                                                                                                AND ( indid IN (
                                                                                                      SELECT  indid
                                                                                                      FROM    sysindexkeys
                                                                                                      WHERE   ( id = a.id )
                                                                                                              AND ( colid IN (
                                                                                                              SELECT
                                                                                                              colid
                                                                                                              FROM
                                                                                                              syscolumns
                                                                                                              WHERE
                                                                                                              ( id = a.id )
                                                                                                              AND ( name = a.name ) ) ) ) ) ) )
                                                                                    AND ( xtype = 'PK' )
                                                                        ) > 0 THEN '√'
                                                                   ELSE ''
                                                              END ) AS primarykey ,
                                                            b.name AS [types] ,
                                                            COLUMNPROPERTY(a.id,a.name,'PRECISION') as  lengths,
                                                            ( CASE WHEN a.isnullable = 1 THEN '√'
                                                                   ELSE ''
                                                              END ) AS ornull ,
                                                            ISNULL(e.text, '') AS [defaults] ,
                                                            ISNULL(g.[value], '') AS describe
                                                     FROM   syscolumns a
                                                            LEFT JOIN systypes b ON a.xtype = b.xusertype
                                                            INNER JOIN sysobjects d ON a.id = d.id
                                                                                       AND d.xtype = 'U'
                                                                                       AND d.name <> 'dtproperties'
                                                            LEFT JOIN syscomments e ON a.cdefault = e.id
                                                            LEFT JOIN sys.extended_properties g ON a.id = g.major_id
                                                                                                   AND a.colid = g.minor_id
                                                     WHERE  d.name = '{tableName}'
                                                     ORDER BY a.id ,
                                                            a.colorder;
                                                ");

            return Json(tableInfo.Select(x => new
            {
                x.fieldname,
                x.identifying,
                x.describe,
                defaults = x.defaults.Replace("(", "").Replace(")", "").Replace("'", ""),
                x.primarykey,
                x.ornull,
                types = x.types.ToLower().Contains("char") ? $"{x.types}({x.lengths})" : x.types,
            }));
        }

        public IHttpActionResult EditTableDescribe(string db, string table, string describe)
        {
            // todo 因为无法判断当前是否已有描述 出此下策 目前是先执行更新出现异常再尝试添加
            try
            {
                // 执行 更新
                DbClient.Excute($@"USE {db}
                                  EXEC sp_updateextendedproperty @name = N'MS_Description',
                                                                 @value = N'{describe}',
                                                                 @level0type = N'user',
                                                                 @level0name = N'dbo',
                                                                 @level1type = N'table',
                                                                 @level1name = N'{table}';");
            }
            catch (Exception)
            {
                // 执行 添加
                DbClient.Excute($@"USE {db}
                                  EXEC sp_addextendedproperty @name = N'MS_Description',
                                                                 @value = N'{describe}',
                                                                 @level0type = N'user',
                                                                 @level0name = N'dbo',
                                                                 @level1type = N'table',
                                                                 @level1name = N'{table}';");
            }
            return Content(HttpStatusCode.OK, "更新成功");
        }
    }
}
