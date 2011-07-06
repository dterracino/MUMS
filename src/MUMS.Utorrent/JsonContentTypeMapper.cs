using System.ServiceModel.Channels;

namespace MUMS.Utorrent
{
    public class JsonContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            if (contentType == null)
                return WebContentFormat.Default;
            
            switch (contentType.ToLower())
            {
                case "text/plain":
                case "text/javascript":
                    return WebContentFormat.Json;
                case "text/xml":
                    return WebContentFormat.Xml;
                case "text/html":
                    return WebContentFormat.Raw;
            }

            return WebContentFormat.Default;
        }
    }
}
