gp = {}
--keyVer = "00";
--securityLevel = "01";
--enc = "404142434445464748494A4B4C4D4E4F"
--mac = "404142434445464748494A4B4C4D4E4F"
--dek = "404142434445464748494A4B4C4D4E4F"
SW9000 = "9000"
ver = "00"
sl = "03"
counter=0;
isd="";

FOR_PERSONALIZATION = 32;
FOR_EXTRADITION = 16;
FOR_MAKE_SELECTABLE = 8;
FOR_INSTALL = 4;
FOR_INSTALL_SELECTABLE = 12;
FOR_LOAD = 2;
FOR_REGISTRY_UPDATE = 64;

DGI8000 = "8000 30 112233440066778811223344550077888B4F854F0831FBF2635A212E4DDDB92A11220044556677881122330055667788";
DGI9000 = "9000 09 97DCB0CE4E2CB37DF3";

DGI8010 = "8010 08 26123456FFFFFFFF";
DGI9010 = "9010 02 0303";

DGIA001 = "A001 3F 15010000FF028016010000FF018017010000FF040018010000FF020019010000FF04001A010000FF00001B010000FF01801C010000FF01801E020000301E30";
DGI8020 = "8020 90 DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546DC5785C8C4B691F41C4A80B062328546";
DGI9020 = "9020 1B 7FDA1C7FDA1C7FDA1C7FDA1C7FDA1C7FDA1C7FDA1C7FDA1C7FDA1C";

function pad(str)
	str = str.."80"
	while(string.len(str)%16 ~=0)
	do
		str = str.."00"
	end	
	return str;
end

function secAPDU(APDU,EXPECT)
	local apdu = string.gsub(APDU," ","");
	local expect = string.gsub(EXPECT," ","");	
	local res = secApdu(apdu);
	local sw = string.sub(res,-4,-1);
	
	if(string.sub(res,1,2)=="6C")
	then
		res = secApdu(string.sub(apdu,1,8)..string.sub(res,3,4));	
		sw = string.sub(res,-4,-1);
	elseif(string.sub(res,1,2)=="61")
	then
		res = send("00C00000"..string.sub(res,3,4));
		sw = string.sub(res,-4,-1);
	end		
	
	if(expect == "")
	then	return;
	end
	
	if( string.len(expect)==4 )
	then
		if( expect ~= sw )
		then error("expect:"..EXPECT);
		end
	elseif( expect ~= res )
	then error("expect:"..EXPECT);
	end	
	
end

function gp.reset()
  counter=0;
  reset();
end

function gp.setKey(enc,mac,dek)
  setKey(enc,mac,dek);
end

function gp.init(keyVer)
  init(keyVer)
end

function gp.ext(securityLevel)
  ext(securityLevel)
end

function gp.apdu(apdu,sw)
  secAPDU(apdu,sw);
end

function gp.getDekSK()
   return getSKdek();
end



function gp.store_data(P1, DGI)
    local dgi = string.gsub(DGI," ","");
	local p1 = P1;
    local p2 = counter;

    dgi4 = string.sub(dgi,1,4)
    if(dgi4=="8201" or dgi4=="8202" or dgi4=="8203" or dgi4=="8204" or dgi4=="8205")
	then
        data = string.sub(dgi,7,-1)
		data = pad(data);
        dgi = dgi4..string.format("%02X",string.len(data)/2)..data;
    end

    if ( bitand(P1,0x60) == 0x60)
	then
        dgi6 = string.sub(dgi,1,6)
        data = string.sub(dgi,7,-1)
        dgi = dgi6..triDesECBen(data, getSKdek());
    end

    secAPDU("80E2"..string.format("%02X",p1)..string.format("%02X",p2)..string.format("%02X",string.len(dgi)/2)..dgi,"9000");
    counter = counter + 1;
    if ( bitand(P1,0x80) == 0x80) 
	then
		counter = 0;
	end
	
end

function gp.select (AID,SW)
	counter=0;
	local aid = string.gsub(AID," ","");
	local sw = string.gsub(SW," ","");
	secAPDU("00A40400"..string.format("%02X",string.len(aid)/2)..aid,SW);
end

function delete(P2,DATA,SW)
	local data = string.gsub(DATA," ","");
	secAPDU("80E400"..string.format("%02X",P2)..string.format("%02X",string.len(data)/2)..data,SW);
end

function gp.delete_instance(AID,SW)
	delete(0,"4F"..string.format("%02X",string.len(AID)/2)..AID,SW)
end

function gp.delete_package(AID,SW)
	delete(0x80,"4F"..string.format("%02X",string.len(AID)/2)..AID,SW)
end

function gp.install(p1, p2, DATA, SW) 
	local data = string.gsub(DATA," ","");
    secAPDU("80E6"..string.format("%02X",p1)..string.format("%02X",p2)..string.format("%02X",string.len(data)/2)..data, SW);
end

function gp.get_data(tag,SW)
	secAPDU("80CA"..tag.."00",SW);
end

function gp.put_des_key_set(ver, key1, key2, key3, SW) 
	local temp1 = string.sub(triDesECBen("0000000000000000",key1),1,6); 
	local temp2 = string.sub(triDesECBen("0000000000000000",key2),1,6);
	local temp3 = string.sub(triDesECBen("0000000000000000",key3),1,6);
	local data = ver.."8010"..triDesECBen(key1,getSKdek()).."03"..temp1..
					  "8010"..triDesECBen(key2,getSKdek()).."03"..temp2..
					  "8010"..triDesECBen(key3,getSKdek()).."03"..temp3
	data = string.format("%02X",string.len(data)/2)..data
	
    secAPDU("80D80081"..data, SW);
end

function gp.replace_des_key_set(ver_old,ver, key1, key2, key3, SW) 
	local temp1 = string.sub(triDesECBen("0000000000000000",key1),1,6); 
	local temp2 = string.sub(triDesECBen("0000000000000000",key2),1,6);
	local temp3 = string.sub(triDesECBen("0000000000000000",key3),1,6);
	local data = ver.."8010"..triDesECBen(key1,getSKdek()).."03"..temp1..
					  "8010"..triDesECBen(key2,getSKdek()).."03"..temp2..
					  "8010"..triDesECBen(key3,getSKdek()).."03"..temp3
	data = string.format("%02X",string.len(data)/2)..data
	
    secAPDU("80D8"..ver_old.."81"..data, SW);
end

function gp.put_des_key(ver, key1, SW) 
	local temp1 = string.sub(triDesECBen("0000000000000000",key1),1,6); 	
	local data = ver.."8010"..triDesECBen(key1,getSKdek()).."03"..temp1;
					
	data = string.format("%02X",string.len(data)/2)..data
	
    secAPDU("80D80001"..data, SW);
end

function gp.put_rsa_key(ver, n, e, SW) 
	local data = ver.."A1"..string.format("%02X",string.len(n)/2)..n.."A0"..string.format("%02X",string.len(e)/2)..e;
	--local data = ver.."A1"..string.format("%02X",string.len(n)/2)..n.."A0"..string.format("%02X",string.len(e)/2)..e.."00";				
	data = string.format("%02X",string.len(data)/2)..data
	
    secAPDU("80D80001"..data, SW);
end
