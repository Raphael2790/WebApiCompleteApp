using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace RSS.Business.Utils
{
    [DataContract]
    public class ServiceResult
    {
        [DataMember]
        public bool Success { get; set; }

        public Exception Error { get; set; }

        [DataMember]
        public string ErrorMessage
        {
            get
            {
                return (Error != null) ? Error.Message : null;
            }
            set { }
        }

        private ServiceResultErrorType _errorType = ServiceResultErrorType.NotSet;

        [DataMember]
        public ServiceResultErrorType ErrorType
        {
            get { return this._errorType; }
            set { this._errorType = value; }
        }

        public ServiceResult SetSuccess()
        {
            this.Success = true;
            this.Error = null;
            return this;
        }

        public ServiceResult SetError(Exception ex)
        {
            this.Success = false;
            this.Error = ex;
            if (ex is ApplicationException)
            {
                this.ErrorType = ServiceResultErrorType.Application;
            }
            else
            {
                this.ErrorType = ServiceResultErrorType.Exception;
            }
            return this;
        }

        public ServiceResult SetError(string errorMessage)
        {
            this.Success = false;
            this.SetError(new ApplicationException(errorMessage));
            return this;
        }

        public ServiceResult SetError(string errorMessage, params object[] parameters)
        {
            this.SetError(string.Format(errorMessage, parameters));
            return this;
        }

        public static ServiceResult GetSuccessResult()
        {
            var result = new ServiceResult();
            return result.SetSuccess();
        }

        public static ServiceResult GetErrorResult(string errorMessage)
        {
            var result = new ServiceResult();
            return result.SetError(errorMessage);
        }
    }

    [DataContract]
    public class ServiceResult<T> : ServiceResult
    {
        [DataMember]
        public T Result { get; set; }

        public new ServiceResult<T> SetSuccess()
        {
            base.SetSuccess();
            return this;
        }

        public ServiceResult<T> SetSuccess(T result)
        {
            base.SetSuccess();
            this.Result = result;
            return this;
        }

        public new ServiceResult<T> SetError(Exception ex)
        {
            base.SetError(ex);
            this.Result = default(T);
            return this;
        }

        public new ServiceResult<T> SetError(string errorMessage)
        {
            base.SetError(errorMessage);
            return this;
        }

        public new ServiceResult<T> SetError(string errorMessage, params object[] parameters)
        {
            base.SetError(errorMessage, parameters);
            return this;
        }

        public static ServiceResult<T> GetSuccessResult(T result)
        {
            var reurnResult = new ServiceResult<T>();
            return reurnResult.SetSuccess(result);
        }
    }

    [DataContract]
    public class SearchResult<T> : ServiceResult<T>
    {
        public SearchResult<T> SetSuccess(T result, int totalCount, int pageIndex, int pageCount)
        {
            var hasMorePages = totalCount > pageIndex * pageCount;
            SetData(result, totalCount, pageIndex, hasMorePages);
            return this;
        }

        public SearchResult<T> SetInfiniteScrollSuccess(T result, int totalCount, int pageIndex, int pageCount)
        {
            var hasMorePages = (totalCount > 0) && (totalCount % pageCount == 0);
            SetData(result, totalCount, pageIndex, hasMorePages);
            return this;
        }

        private void SetData(T result, int totalCount, int pageIndex, bool hasMorePages)
        {
            base.SetSuccess();
            this.HasMorePages = hasMorePages;
            this.Result = result;
            this.TotalCount = totalCount;
            this.PageIndex = pageIndex;
            this.NextPageIndex = this.HasMorePages ? ++pageIndex : 0;
        }
        public void SetHasMorePage(bool value)
        {
            this.HasMorePages = value;
        }

        [DataMember]
        public int TotalCount { get; private set; }

        [DataMember]
        public int PageIndex { get; private set; }

        [DataMember]
        public int NextPageIndex { get; private set; }

        [DataMember]
        public bool HasMorePages { get; private set; }

        public new SearchResult<T> SetError(Exception ex)
        {
            base.SetError(ex);
            this.Result = default(T);
            return this;
        }
    }

    public enum ServiceResultErrorType
    {
        NotSet,
        Exception,
        Application
    }
}
