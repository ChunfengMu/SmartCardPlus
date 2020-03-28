人生中第一个工具，不懂什么高级特性(例如事件、委托等)，所以比较敢写，代码也比较简单，也有点混乱。

code部分是源码，使用VS2012以上版本即可(2010应该也可以，我没尝试)，支持[js脚本](https://apdu.github.io/notes/2019/08/01/jsapi.html)

application部分是之前发布的应用程序，不支持js脚本，支持js的版本没发布

代码中也用到了前辈们的模块:[nlua](https://github.com/NLua/NLua)、[pcsc sharp](https://github.com/danm-de/pcsc-sharp)等，可能有些部分也是参考他人的代码。




version:4.0.0.0    
Test passed on Winxp(x86),win7(x86/x64),win8(x86/x64),win10(x64).  

support PCSC/CCID reader.  
support scp01/scp02, security level:No secure / C-MAC / C-DECRYPTION and C-MAC.  
support data authentication pattern (DAP).  
support delegated management (DM) :	GP2.1 / GP2.2 / China Mobile CMS2AC / China Unicom UICC / China Telecom UICC.  
support APDU send & receive.  
support support .txt script / .lua script(lua5.3.5, lua script also support dual reader).   
support cap file download.  
support multi-cap download (.txt).  
support view & install & delete card content:application, executable load files, security domain.  
support get card available memory.  
support KMC diversify: CPG202 / CPG212.  
support base58,base64,des/3des,aes,sha1,sha2,sha3,hmac,rsa(max 16384 bit),PKCS1,ecc(ecdsa),ecc.  
support SM1/SM2/SM3/SM4(SM1 based on hardware ).   
support JCOP Debug.  

[lua script demo](https://github.com/APDU/lua-script)   

[txt script demo](https://github.com/APDU/SmartCardPlus/tree/master/application/script/txt)  

[multi-cap download demo](https://github.com/APDU/SmartCardPlus/tree/master/application/cap)  

[doc](https://github.com/APDU/SmartCardPlus/tree/master/doc)    

[api](http://apdu.github.io/notes/2018/09/01/luaapi.html)   

this is a WPF program,may you need [.NET Framework 4.0+](https://www.microsoft.com/en-us/download/details.aspx?id=17718)  

[My Blog](http://apdu.github.io)   
