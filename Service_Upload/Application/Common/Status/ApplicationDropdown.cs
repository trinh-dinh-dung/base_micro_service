using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Status
{
    public enum GenderDropdown
    {
        Male = 1, // nam
        Female = 2,// nữ
        None = 3,
    }
    public enum MaritalsSatusDropdown
    {
        NotMarried = 1, // chưa kết hôn
        Married = 2,// Đã kết hôn
    }
    public enum UserContactRelationshipDropdown
    {
        Grandfather = 1, //Ông
        Grandmother = 2, // Bà
        Father = 3, // Bố
        Mother = 4, // Mẹ
        Uncle = 5,// Chú
        Aunt = 6,// Cô dì
        Brother = 7, // Anh, Em
        Sister = 8, // Chị, Em
        Child = 9, // Con
        Grandchildren = 10, // cháu
    }
}
