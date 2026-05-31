using System;

namespace Api.Base.JWT
{
    public class UserTokens
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public TimeSpan Validaty { get; set; }
        public string RefreshToken { get; set; }
        public Guid Id { get; set; }
        public string EmailId { get; set; }
        public Guid GuidId { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string MaBv { get; set; }
        public int ExpC { get; set; }
        public int TypeLogin { get; set; }


        public string ma_user { get; set; }
        public string ten_user { get; set; }
        public string ma_donvi { get; set; }
        public string full_name { get; set; }
        public string ma_benhvien { get; set; }
        public int login_fail_number { get; set; }
    }
}
