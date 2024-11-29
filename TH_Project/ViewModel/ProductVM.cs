using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TH_Project.Data;

namespace TH_Project.ViewModel
{
    public class ProductVM
    {
        public IEnumerable<XEGANMAY> XEes { get; set; }
        public IEnumerable<LOAIXE> LOAIXEs { get; set; }
        public IEnumerable<HANGSANXUAT> HSXs { get; set; }
    }
}