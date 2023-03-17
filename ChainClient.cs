
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

    [Function("pull")]
    public class PullFuncation: FunctionMessage
    {
        [Parameter("uint", "_position", 1)]
        public int Position {get;set;}
    }

    [Function("push")]
    public class PushFuncation: FunctionMessage
    {
        [Parameter("string", "_aggregateId", 1)]
        public string? AggregateId { get; set; }

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

        public async Task<IEnumerable<OmniChainEvent>> Pull(int position)
        {
            var handler = _handler.GetFunction<PullFuncation>();
            var request = new PullFuncation{
                Position = position
            };
            var eventsDto = await handler.CallAsync<PullEventsDTO>(request);
            return eventsDto.Events;
        }

        public async Task<string> Push(int position, IEnumerable<OmniChainEvent> events)
        {
            var handler = _handler.GetFunction<PushFuncation>();
            var request = new PushFuncation{
                Events = events.ToList(),
                Position = position,
                AggregateId = "0"
            };
            return await handler.CallAsync<string>(request);
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