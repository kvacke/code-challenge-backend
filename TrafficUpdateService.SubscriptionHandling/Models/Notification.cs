using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficUpdateSubscriptionSystem.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }


        public Notification(string id, string title, string description, string priority, string category)
        {
            Id = id;
            Title = title;
            Description = description;
            Priority = GetPriorityStringFromPriorityLevel(priority);
            Category = GetCategoryStringFromCategoryNumber(category);
        }

        public override string ToString()
        {
            return $"\n\nPrioritetsnivå: {Priority}" +
                   $"\nKategori: {Category}" +
                   $"\nRubrik: {Title}\n" +
                   $"\nBeskrivning: {Description}\n";
        }

        private string GetPriorityStringFromPriorityLevel(string level)
        {
            switch (level)
            {
                case "1": return "Mycket allvarlig händelse";
                case "2": return "Stor händelse";
                case "3": return "Störning";
                case "4": return "Information";
                case "5": return "Mindre störning";
                default: return "Saknar prioritetsnivå";
            }
        }

        private string GetCategoryStringFromCategoryNumber(string category)
        {
            switch (category)
            {
                case "0": return "Vägtrafik";
                case "1": return "Kollektivtrafik";
                case "2": return "Planerad störning";
                case "3": return "Övrigt";
                default: return "Saknar kategori";
            }
        }
    }
}
