using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Exception Exception { get; }

        protected Result(Exception ex)
        {
            Exception = ex;
            IsSuccess = false;
        }

        protected Result()
        {
            IsSuccess = true;
        }

        public static Result Success()
        {
            return new Result();
        }

        public static Result Fail(Exception ex = null)
        {
            return new Result(ex);
        }
    }

    public class Result<TData> : Result where TData : class
    {
        public TData Data { get; }

        private Result(TData data) : base()
        {
            Data = data;
        }

        private Result(Exception ex) : base(ex)
        {
        }

        public static Result<TData> Success(TData data = null)
        {
            return new Result<TData>(data);
        }

        public new static Result<TData> Fail(Exception ex = null)
        {
            return new Result<TData>(ex);
        }
    }
}
