using Cila.OmniChain;

namespace Cila
{

    public class RelayService
    {
        public string Id { get; private set; }
        
        //private List<DomainEevent> _eventstore;

        private List<IExecutionChain> _chains;

        public RelayService(OmniChainRelaySettings config)
        {
            _chains = new List<IExecutionChain>();
            Id = config.RelayId;
            foreach (var item in config.Chains)
            {
            
                var chain1 = new ExecutionChain();
                chain1.ChainService = new EthChainClient(item.Rpc,item.Contract,item.PrivateKey);
            }
        }

        public void SyncAllChains()
        {
            //fetch the latest state for each chains
            foreach (var chain in _chains)
            {
                chain.Update();
            }
            var leaderEventNumber = _chains.Max(x=> x.Length);
            var leader = _chains.Single(x=> x.Length == leaderEventNumber);
            foreach (var chain in _chains)
            {
                if (chain.ID == leader.ID)
                {
                    continue;
                }
                var newEvents = leader.GetNewEvents(chain.Length); 
                chain.PushNewEvents(newEvents);
            }
        }
    }
}