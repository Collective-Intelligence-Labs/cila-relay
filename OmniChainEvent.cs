using Nethereum.ABI.FunctionEncoding.Attributes;


namespace Cila 
{
    public enum DomainEventType {
        NFTMinted = 0,
        NFTTransfered = 1
    }
    public class OmniChainEvent
    {
        [Parameter("int", "t", 1)]
        public DomainEventType EventType {get;set;}

        [Parameter("bytes", "payload", 2)]
        public byte[] Payload {get;set;}

        [Parameter("int", "eventNumber", 3)]
        public int EventNumber { get; set; }

    }
}
