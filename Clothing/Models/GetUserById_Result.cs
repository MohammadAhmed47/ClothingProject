//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Clothing.Models
{
    using System;
    
    public partial class GetUserById_Result
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<int> Role { get; set; }
    }
}