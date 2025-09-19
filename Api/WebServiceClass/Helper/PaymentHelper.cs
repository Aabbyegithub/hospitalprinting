using WebServiceClass.Helper.WeChat;
using WebServiceClass.Helper.Alipay;
//using WebServiceClass.Helper.Bank;

namespace WebServiceClass.Helper
{
    /// <summary>
    /// 统一支付入口
    /// </summary>
    public class PaymentHelper
    {
        #region 微信支付
        public bool WeChatPay_Code(string orderNo, decimal amount, string authCode)
            => WeChatPayHelper.CodePay(orderNo, amount, authCode);

        public string WeChatPay_Native(string orderNo, decimal amount, string productDesc)
            => WeChatPayHelper.NativePay(orderNo, amount, productDesc);

        public Dictionary<string, string> WeChatPay_JSAPI(string orderNo, decimal amount, string openId, string productDesc)
            => WeChatPayHelper.JsApiPay(orderNo, amount, openId, productDesc);
        #endregion

        #region 支付宝支付
        public string Alipay_TradeWapPay(string orderNo, decimal amount, string subject, string returnUrl, string notifyUrl)
            => AlipayPayHelper.WapPay(orderNo, amount, subject, returnUrl, notifyUrl);

        public string  Alipay_TradeAppPay(string orderNo, decimal amount, string subject, string notifyUrl)
            => AlipayPayHelper.AppPay(orderNo, amount, subject, notifyUrl);

        public string AliPayBarcode(string orderNo, decimal amount, string subject, string authCode)
            => AlipayPayHelper.BarcodePay(orderNo, amount, subject, authCode);
        #endregion

        #region 银行卡支付
        //public BankPaymentResult PayByBankCard(string orderNo, decimal amount, BankCardPaymentParam param)
        //    => BankPayHelper.Pay(orderNo, amount, param);
        #endregion
    }
}
