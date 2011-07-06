using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace MUMS.Utorrent.Service
{
    [ServiceContract]
    public interface IUtorrentChannel
    {
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/?token={token}&list=1")]
        ListResponse GetList(string token);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/token.html")]
        Stream GetToken();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/?token={token}&action=add-url&s={torrentUrl}&cookie={cookieString}")]
        DefaultResponse AddTorrentFromUrl(string token, string torrentUrl, string cookieString);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/?token={token}&action=setprops&hash={torrentHash}&s={name}&v={value}")]
        DefaultResponse SetProperty(string token, string torrentHash, string name, string value);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/?token={token}&action=removedata&hash={torrentHash}")]
        DefaultResponse RemoveTorrentAndData(string token, string torrentHash);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/?token={token}&action=remove&hash={torrentHash}")]
        DefaultResponse Remove(string token, string torrentHash);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/?token={token}&action=start&hash={torrentHash}")]
        DefaultResponse Start(string token, string torrentHash);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/?token={token}&action=stop&hash={torrentHash}")]
        DefaultResponse Stop(string token, string torrentHash);
    }
}
