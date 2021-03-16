namespace SyzunaPrograms.ExtensionBackendExample.Models
{
    public class ExtensionPubsubMessage
    {
        public string[] Targets { get; set; }
        public string ChannelId { get; set; }
        public object Message { get; set; }
    }
}