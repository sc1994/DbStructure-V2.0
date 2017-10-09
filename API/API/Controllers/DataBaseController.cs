using System.Linq;
using System.Web.Http;
using API.Common;
using API.Models;

namespace API.Controllers
{
    public class DataBaseController : ApiController
    {
        /// <summary>
        /// 获取全部或指定的库(DB)信息
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllDataBase(string dbName)
        {
            var dbList = DbClient.Query<string>($@"SELECT name
                                                  FROM   master.dbo.sysdatabases
                                                  WHERE  name NOT IN ( 'master', 'tempdb', 'model', 'msdb','ReportServer','ReportServerTempDB' ) 
                                                  {(dbName.IsNullOrEmpty() ? "" : $" AND name = '{dbName}'")}
                                                  ORDER BY name;").ToList();

            var nodeList = dbList.Select(x => new NodeModel
            {
                id = "1-" + dbList.IndexOf(x).ToString(),
                label = x
            });
            return Json(nodeList);
        }

        /// <summary>
        /// 获取指定库下面的全部表信息
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetTablesByDb(string dbName)
        {
            var tableList = DbClient.Query<TableBaseModel>($@"USE {dbName}
                                                      SELECT  TableName = CASE WHEN a.colorder=1 THEN d.name
                                                                          ELSE ''
                                                                     END,TableDescribe = CASE WHEN a.colorder=1 THEN ISNULL(f.value,'')
                                                                                    ELSE ''
                                                                               END
                                                        FROM    syscolumns a
                                                        LEFT   JOIN systypes b
                                                                ON a.xusertype=b.xusertype
                                                        INNER   JOIN sysobjects d
                                                                ON a.id=d.id
                                                                   AND d.xtype='U'
                                                                   AND d.name<>'dtproperties'
                                                        LEFT   JOIN syscomments e
                                                                ON a.cdefault=e.id
                                                        LEFT   JOIN sys.extended_properties g
                                                                ON a.id=g.major_id
                                                                   AND a.colid=g.minor_id
                                                        LEFT   JOIN sys.extended_properties f
                                                                ON d.id=f.major_id
                                                                   AND f.minor_id=0 
                                                        ORDER   BY a.id,a.colorder;").ToList();
            var nodeList = tableList
                .Where(x => !x.TableName
                    .Trim()
                    .IsNullOrEmpty())
                    .Select(x => $"{x.TableName}{(string.IsNullOrEmpty(x.TableDescribe) ? "" : $"({x.TableDescribe})")}");
            return Json(nodeList);
        }
    }
}
