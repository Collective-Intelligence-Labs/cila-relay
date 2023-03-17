public class OmniChainEvent
{
    public string ChainID {get;set;}

    public byte[] Payload {get;set;}

    public int BLockNumber {get;set;}

    public string AggregateID {get;set;}

    public int EventNumber {get;set;}

    public int EventType {get;set;}

    public string EventHash {get;set;}

}