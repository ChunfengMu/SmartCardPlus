
for i=1,10000,1 
do 
	print("*********** "..i.."***********");
    reset();
	send("00BC010000","9000");
	local p = send("00DC020100","9000");
	p = string.sub(p,0,string.len(p)-4)
	local d = send("00DC020200","9000");
	d = string.sub(d,0,string.len(d)-4)

	local dd = send("00BC020000","9000");
	dd = string.sub(dd,0,string.len(dd)-4)
	--print("dd "..dd);
	--print("d "..d);
	if(dd ~= p)
	then	error("get pub key fail");
	else    print("get pub key pass");
	end
	local data = "5648D2F386DB69BEE956A9AEC04983B5";
	local apdu = "00BC0300" .. string.format("%02X",string.len(data)/2) .. data;
	local res = send(apdu,"9000");
	res = string.sub(res,0,string.len(res)-4)
	local res2 = sm2DeC1C3C2(res,d);

	if(data ~= res2)
	then	error("enc fail: " .. res2);
	else    print("enc pass");
	end
end

