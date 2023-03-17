
using Nethereum.Web3;
using Nethereum.Contracts;

using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;

namespace Cila.OmniChain
{
    interface IChainClient
    {
        void Send(OmniChainOperation op);
        void Push(IEnumerable<OmniChainEvent> events);
        IEnumerable<OmniChainEvent> Pull(int position);
    }

    public class OmniChainOperation
    {
    }

    [Function("pull")]
    public class PullFuncation: FunctionMessage
    {
        [Parameter("uint", "_position", 1)]
        public int Position {get;set;}
    }

    public class EthChainClient : IChainClient
    {
        private Web3 _web3;
        private Contract _contract;
        private string _privateKey;

        public EthChainClient(string rpc, string contract, string abi, string privateKey)
        {
            
            _privateKey = privateKey;
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            _web3 = new Web3(account, rpc);
            _contract = _web3.Eth.GetContract(abi,contract);
        }

        public async Task<IEnumerable<OmniChainEvent>> Pull(int position)
        {
            var pullEvents = _contract.GetFunction("pull");
            var eventsDto = await pullEvents.CallAsync<PullEventsDTO>(position);
            return eventsDto.Events;
        }

        public async Task<string> Push(IEnumerable<OmniChainEvent> events)
        {
            var dispatchFunc = _contract.GetFunction("push");
            var result = await dispatchFunc.CallAsync<string>(events);
            return result;
        }

        IEnumerable<OmniChainEvent> IChainClient.Pull(int position)
        {
            return Pull(position).GetAwaiter().GetResult();
        }

        void IChainClient.Push(IEnumerable<OmniChainEvent> events)
        {
            Push(events).GetAwaiter().GetResult();
        }

        async Task Send(OmniChainOperation op)
        {
            var operation = new DispatchOperation {

                Payload = op
            };
            var dispatchOperationHandler = _contract.GetFunction<DispatchOperation>();
            var result = await dispatchOperationHandler.CallAsync(operation); 
        }

        void IChainClient.Send(OmniChainOperation op)
        {
            Send(op).GetAwaiter().GetResult();
        }
    }

    internal class DispatchOperation: CallInput
    {
        [Parameter("bytes", "_data", 1)]
        public object Payload { get; set; }
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