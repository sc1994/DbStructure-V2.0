using System.Collections;
using System.Web.Http;
using API.Common;
using API.Models;

namespace API.Controllers
{
    public class DataController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetDataList(string sql)
        {
            var list = DbClient.Query<dynamic>(sql);
            return Json(list);
        }

        public IHttpActionResult GetDataHead(string db, string table)
        {
            var head = new ArrayList();
            var tableInfo = DbClient.Query<TableInfo>($@"USE {db};
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
                                                     WHERE  d.name = '{table}'
                                                     ORDER BY a.id ,
                                                            a.colorder;
                                                ");
            foreach (var t in tableInfo)
            {
                var orderKey = !t.identifying.IsNullOrEmpty() || !t.primarykey.IsNullOrEmpty();
                head.Add(new
                {
                    title = t.fieldname,
                    key = t.fieldname,
                    width = GetWidth(t.types, t.lengths),
                    ellipsis = true,
                    sortable = true,
                    orderKey
                });
            }
            return Json(head);
        }

        private int GetWidth(string type, string length)
        {
            type = type.ToLower();
            length = length.ToLower();
            if (type == "int" ||
                type == "tinyint" ||
                type == "smallint" ||
                type == "bigint" ||
                type == "decimal" ||
                type == "smallmoney" ||
                type == "money" ||
                type == "float" ||
                type == "decimal" ||
                type == "char")
            {
                return 150;
            }
            if (type == "datetime")
            {
                return 240;
            }
            if (length.ToInt() > 200 && length.ToInt() < 500)
            {
                return (length.ToDouble() / 1.6).ToInt();
            }
            if (length.ToInt() > 500)
            {
                return 320;
            }
            return 240;
        }
    }
}
