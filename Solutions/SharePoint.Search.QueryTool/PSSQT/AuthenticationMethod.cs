using System;

namespace PSSQT
{
    public enum PSAuthenticationMethod
    {
        CurrentUser,
        Windows,
        SPO,
        SPOManagement   
     }

    public class PSAuthenticationMethodFactory
    {
        public static PSAuthenticationMethod? DefaultAutenticationMethod()
        {
            string defaultAuthMethod = Environment.GetEnvironmentVariable("PSSQT_DefaultAuthenticationMethod");

            if (!String.IsNullOrWhiteSpace(defaultAuthMethod))
            {
                PSAuthenticationMethod value = (PSAuthenticationMethod) Enum.Parse(typeof(PSAuthenticationMethod), defaultAuthMethod);

                return value;
            }


            return null;
        }
    }
}