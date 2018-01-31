package.path =string.format("%s\\lib\\?.lua", getDir())
require("pboclib")

AID = "A0000003330101";
--AID = "A000000632010106";

Reset();
selectPSE();
selectPBOC();
--tag9F66 = "A6000000"
GPO();
--VerifyPIN("123456");
GAC1("80");
IssuerAuth ("3030")
GAC2("40");
Script("04DA 9F79 0A 000000011000");

