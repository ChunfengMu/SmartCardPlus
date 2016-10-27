package.path =string.format("%s\\lib\\?.lua", getDir())
require("lib2")
lib2print();

reset();
local temp = send("00A4040000","");
print(temp);
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","");
send("00A4040000","9001");