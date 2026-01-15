using System;
using System.Collections.Generic;
using System.Text;

namespace ElectricStoreProject.Application.DTOs.Response
{
    public class CommonBaseResponse
    {

        public Guid? Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
