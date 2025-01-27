﻿
// ReSharper disable InconsistentNaming
namespace SimpleCL.Enums.Commons {
    /// <summary>
    /// Specifies what local was this version of Silkroad released for. Taken from Silkroad CertServer.
    /// </summary>
    public enum Locale: byte {
        Silkroad_Dev = 1,
        Silkroad_Korea_Yahoo_Official = 2,
        Silkroad_Korea_Yahoo_Test_IN = 3,
        SRO_China_Official = 4,
        SRO_China_TestLocal = 5,
        Silkroad_Joymax = 6,
        JoymaxMessenger = 7,
        ServiceManager = 8,
        SRO_China_TestIn = 9,
        SRO_Taiwan_TestIn = 10,
        SRO_Taiwan_TestLocal = 11,
        SRO_Taiwan_Official = 12,
        SRO_DEEPDARK = 13,
        SRO_Taiwan_BillingTest = 14,
        SRO_Japan_Official = 15,
        SRO_Japan_TestLocal = 16,
        SRO_Japan_TestIn = 17,
        SRO_Global_TestBed = 18,
        SRO_Global_TestBed_In = 19,
        SRO_EuropeTest = 20,
        SRO_Vietnam_TestIn = 21,
        SRO_Vietnam_TestLocal = 22,
        SRO_Net2E_Official = 23,
        Yahoo_Official_Test = 24,
        SRO_GNGWC_TestIn = 25,
        SRO_GNGWC_Official = 26,
        SRO_China_OpenTest = 27,
        SRO_GNGWC_Official_Final = 29,
        CPRJ_Dev = 30,
        SRO_INTERNAL_EU = 31,
        SRO_INTERNAL_EU_QUEST = 32,
        Vietnam_Dev = 33,
        SRO_China_EuroTest = 34,
        SRO_Taiwan_FOS_CB = 35,
        SRO_GameOn_Official_Test = 36,
        SRO_Thailand_TestLocal = 37,
        SRO_Thailand_Official = 38,
        SRO_Russia_TestLocal = 39,
        SRO_Russia_Official = 40,
        SRO_Japan_TestOTP = 41,
        SRO_Global_TestBed_OT = 42,
        SRO_Japan_CGI_TestIn = 43,
        SRO_Japan_TestLocal_We = 44,
        SRO_R_JP_TestLocal_We = 45,
        SRO_R_JP_RealLocal_We = 46,
        SRO_R_CH_TestLocal_CIMO = 47,
        SRO_R_CH_RealLocal_CIMO = 48,
        SRO_TR_Official_GameGami = 56,
    }

    static class Extensions
    {
        public static bool IsInternational(this Locale locale)
        {
            return locale is Locale.SRO_TR_Official_GameGami or Locale.SRO_Global_TestBed;
        }
    }
}