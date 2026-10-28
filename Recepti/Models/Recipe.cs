using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace Recepti
{
    public class Recipe
    {
        public int Id { get; set; }     
        public string Title { get; set; }        
        public string Category { get; set; }   
        public int Servings { get; set; }
        public string ImagePath { get; set; }  
        public string RtfPath { get; set; }     
        public DateTime DateAdded { get; set; }  

        public bool IsSelected { get; set; }     

      
    }
   
}
