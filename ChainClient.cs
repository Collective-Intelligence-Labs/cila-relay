
using Nethereum.Web3;
using Nethereum.Contracts;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Contracts.ContractHandlers;

namespace Cila.OmniChain
{
    interface IChainClient
    {
        void Push(int position, IEnumerable<OmniChainEvent> events);
        IEnumerable<OmniChainEvent> Pull(int position);
    }

    [Function("get")]
    public class PullFuncation: FunctionMessage
    {
        [Parameter("address", "aggregateId", 1)]
        public string AggregateId {get;set;}

        [Parameter("uint", "startIndex", 2)]
        public int StartIndex {get;set;}

        [Parameter("uint", "limit", 3)]
        public int Limit {get;set;}
    }

    [Function("sync")]
    public class PushFuncation: FunctionMessage
    {
        [Parameter("address", "_aggregateId", 1)]
        public string AggregateId { get; set; }

        [Parameter("uint", "_position", 2)]
        public int Position {get;set;}

        [Parameter("DomainEvent[]", "events", 3)]
        public List<OmniChainEvent>? Events {get;set;}
    }

    public class EthChainClient : IChainClient
    {
        private Web3 _web3;
        private ContractHandler _handler;
        private string _privateKey;

        public EthChainClient(string rpc, string contract, string privateKey)
        {
            
            _privateKey = privateKey;
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            _web3 = new Web3(account, rpc);
            _handler = _web3.Eth.GetContractHandler(contract);  
        }

        public const int MAX_LIMIT = 1000000;
        public const string AGGREGATE_ID = "0x4215a6F868D07227f1e2827A6613d87A5961B5f6";


        public async Task<IEnumerable<OmniChainEvent>> Pull(int position)
        {
             Console.WriteLine("Chain Service Pull execution started from position: {0}, aggregate: {1}", position, AGGREGATE_ID);
            var handler = _handler.GetFunction<PullFuncation>();
            var request = new PullFuncation{
                StartIndex = position,
                Limit = MAX_LIMIT,
                AggregateId = AGGREGATE_ID
            };
                var result =  await handler.CallAsync<List<OmniChainEvent>>(request);
                Console.WriteLine("Chain Service Pull executed: {0}", result);
                return result;

            //return eventsDto.Events;
        }

        public async Task<string> Push(int position, IEnumerable<OmniChainEvent> events)
        {
            var handler = _handler.GetFunction<PushFuncation>();
            var request = new PushFuncation{
                Events = events.ToList(),
                Position = position,
                AggregateId = "0"
            };
            var result = await handler.CallAsync<string>(request);
            Console.WriteLine("Chain Service Push} executed: {0}", result);
            return result;
        }

        IEnumerable<OmniChainEvent> IChainClient.Pull(int position)
        {
            return Pull(position).GetAwaiter().GetResult();
        }

        void IChainClient.Push(int position, IEnumerable<OmniChainEvent> events)
        {
            Push(position,events).GetAwaiter().GetResult();
        }
    }

    [FunctionOutput]
    public class PullEventsDTO: IFunctionOutputDTO
    {
        [Parameter("uint256", "position", 1)]
        public BigInteger Position {get;set;}

        [Parameter("DomainEvent[]", "events", 2)]
        public List<OmniChainEvent> Events {get;set;}
    }
}