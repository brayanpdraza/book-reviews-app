using AdaptadorAPITest.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptadorAPITest.Contollers.UserController
{
    public class CustomWebApplicationFactoryHelper
    {

        internal CustomWebApplicationFactory<Program> Factory { get; private set; }

        public CustomWebApplicationFactoryHelper()
        {
            Factory = new CustomWebApplicationFactory<Program>();
        }

    }
}
