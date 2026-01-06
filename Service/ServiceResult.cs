using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { IsSuccess = true, Data = data };
        public static ServiceResult<T> Failure(string msg) => new ServiceResult<T> { IsSuccess = false, Message = msg };
    }
}
