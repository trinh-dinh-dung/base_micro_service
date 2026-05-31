namespace Application.GetMap
{
    public class ResponseApi
    {
        public ResponseApi()
        {
            isSuccess = true;
            message = "Successful";

        }
        public object data { get; set; }
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public ResponseApi(object Data, bool IsSuccess, string Mess = "", int Status = 0)
        {
            if (IsSuccess == true && string.IsNullOrEmpty(Mess)) Mess = "Successful";
            else Mess = "Fail";
            data = Data;
            isSuccess = IsSuccess;
            message = Mess;
            status = Status;
        }
    }
    public class ResultApp
    {
        public object Data { get; set; }
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string MesageStatus { get; set; }
    }
}
