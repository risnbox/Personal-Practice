using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;

namespace Asp.net_Exercise.WebApi.BackStage
{
    public class AnalyticsAPIController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Test(string start, string end, string dimension, string metric)
        {
            string[] startdate = start.Split(',');//可接受不只單一日期 使用,分隔
            string[] enddate = end.Split(',');
            string[] metricdata = metric.Split(',');
            string[] dimensionData = dimension.Split(',');
            List<DateRange> dateranges = new List<DateRange>();
            List<Metric> Metrics = new List<Metric>();
            List<Dimension> dimensions = new List<Dimension>();
            for(int i = 0; startdate.Length > i; i++)
            {
                var date = new DateRange()
                {
                    StartDate = startdate[i],
                    EndDate = enddate[i]
                };
                dateranges.Add(date);
            }
            foreach(var x in metricdata)
            {
                var data = new Metric() { Expression = x };
                Metrics.Add(data);
            }
            foreach(var t in dimensionData)
            {
                var data = new Dimension() { Name = t };
                dimensions.Add(data);
            }
            //更新Google授權物件
            var json = "";
            //因為用雲端無法直接從本地提取金鑰改用雲端本身的blob來存取
            using(HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage httpResponse = await httpClient.GetAsync("https://aspnetblobtest.blob.core.windows.net/testblob/innate-beacon-276109-76330b72b504.json");
                    json = await httpResponse.Content.ReadAsStringAsync();
                }
                catch(Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            var googleCredential = GoogleCredential.FromJson(json).CreateScoped(AnalyticsReportingService.Scope.Analytics);
            //建立查詢用物件
            var analyticsReporting = new AnalyticsReportingService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = googleCredential,
                ApplicationName = "GA"
            });
            //建立查詢條件List(因參數需要List),可單次多重查詢
            List<ReportRequest> requests = new List<ReportRequest>()
            {
                new ReportRequest
                {
                    ViewId = "251956256",
                    Metrics = Metrics,
                    Dimensions = dimensions,
                    DateRanges = dateranges
                } 
            };
            try
            {
                //建立查詢請求本體物件
                var reportGetRequest = new GetReportsRequest { ReportRequests = requests };
                //送出查詢後取得回應
                List<GetReportsResponse> gets = new List<GetReportsResponse>() { analyticsReporting.Reports.BatchGet(reportGetRequest).Execute() };
                return Ok(gets);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
