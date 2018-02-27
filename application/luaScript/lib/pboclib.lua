UDK = "11223344006677881122334455007788"
MAC_UDK = "8B4F854F0831FBF2635A212E4DDDB92A"
ENC_UDK = "11220044556677881122330055667788"


AID = "A0000003330101"
--ATC
tag9F36 = ""
--PDOL
tag9F38 = ""
--Log Entry
tag9F4D = ""
--Log Format
tag9F4F = ""
--AIP
tag82 = ""
--AFL
tag94 = ""
--CDOL1
tag8C = ""
--CDOL2
tag8D = ""
--APPLICATION CIPHER
tagAC1 = ""

--TERMINAL DATA

--Terminal_Transaction_Qualifiers
tag9F66 = "40000000"
--Transaction_amount
tag9F02 = "000000000001"
--Other_amount
tag9F03 = "000000000000"
--Terminal_Country_Code
tag9F1A = "0156"
--Terminal_Verify_Resault
tag95 = "0000000000"
--Transaction_Currency_Code
tag5F2A = "0156"
--Transaction_Date
tag9A = "160228"
--Transaction_Time
tag9F21 = "080808"
--Transaction_Type
tag9C = "40"
--Terminal_Unpredictable_Number
tag9F37 = "11223344"
--Terminal_Capabilities 
tag9F33 = "E0E9E8"
--CAPP_Transaction_Flag
tagDF60 = "00"
--Authorisation_Response_Code
tag8A = "3030"
--Merchant_Name
tag9F4E = "5261756C2058752D476F6C647061632032303035"
--EC_Terminal_Support_Indicator
tag9F7A = "00"
--SM2_ALG_Indicator
tagDF69 = "00"


function getBytes (str, start, length)
	return string.sub(str,start,start+length-1);
end

function Reset () 
	reset();
end

function Send (apdu,sw)
	local res = send(apdu,"");
	if(string.sub(res,1,2)=="6C")
	then
		res = send(string.sub(apdu,1,8)..string.sub(res,3,4),sw);	
	elseif(string.sub(res,1,2)=="61")
	then
		res = send("00C00000"..string.sub(res,3,4),sw);
	elseif(string.sub(res,-4) ~= sw)
	then
		error("error,sw should be"..sw);
		
	end	
	return res;	
end

function selectPPSE ()
	local res = Send("00A404000E325041592E5359532E4444463031","9000");
		
	local offset = string.find (res,"4F");

	if (offset == nil) 
	then	
		error("tag4F not founded");
	else 
		local len = getBytes(res, offset+2, 2);	
		len = tonumber(len,16);
		AID = getBytes(res, offset+4, len*2);
		--print("AID:"..AID);
	end
end


function selectPSE ()
	local res = Send("00A404000E315041592E5359532E4444463031","9000");
		
	local offset = string.find (res,"88");

	if (offset == nil) 
	then	
		error("tag88 not founded");
	else 
		temp = getBytes(res, offset+4, 2);
		local itemp = tonumber(temp,16)	
		res = Send("00B201"..string.format("%02X",lshift(itemp,3)+4).."00","9000");
		offset = string.find (res,"4F");
		if (offset == nil) 
		then	
			error("tag4F not founded");
		else 
			local len = getBytes(res, offset+2, 2);	
			len = tonumber(len,16);
		AID = getBytes(res, offset+4, len*2);
		--print("AID:"..AID);
		end
	
	end
end


function selectPBOC ()
	local res = Send("00A40400" .. string.format("%02X",string.len(AID)/2) .. AID,"9000");
					
	local offset = string.find (res,"9F38");
	if (offset  == nil)
	then
		error("tag9F38 not founded");			
	else  
		local len = getBytes(res, offset+4, 2);	
		len = tonumber(len,16);
		tag9F38 = getBytes(res, offset+6, len*2);
		--print("PDOL:"..tag9F38);
	end
		
end

function  getTag (tag)
	if(tag == "9F66")
	then	
		return tag9F66
	elseif(tag == "9F02")
	then			
		return tag9F02
	elseif(tag == "9F03")
	then	
		return tag9F03
	elseif(tag == "9F1A")
	then
		return tag9F1A
	elseif(tag == "95")
	then	
		return tag95
	elseif(tag == "5F2A")
	then	
		return tag5F2A
	elseif(tag == "9A")
	then	
		return tag9A
	elseif(tag == "9F21")
	then	
		return tag9F21
	elseif(tag == "9C")
	then	
		return tag9C
	elseif(tag == "9F37")
	then	
		return tag9F37
	elseif(tag == "9F33")
	then	
		return tag9F33
	elseif(tag == "DF60")
	then	
		return tagDF60
	elseif(tag == "8A")
	then	
		return tag8A	
	elseif(tag == "9F4E")
	then	
		return tag9F4E
	elseif(tag == "9F7A")
	then	
		return tag9F7A
	elseif(tag == "DF69")
	then	
		return tagDF69
	else 
			error  "tag not founded"
			return ""
		
	end
	
end



function MakeDOL (dol)
	local i = 1
	local res = ""
	local flag = 0
	while (i < string.len(dol) )
	do
		local temp = string.sub(dol,i,i+1);
		local itemp = tonumber(temp,16)		
		if ( bitand(itemp,0x1f) == 0x1f ) 
		then  flag = 1
		else
			 flag = 0
		end
		
		local tag = string.sub(dol,i,i+2*flag+1);		
		local data = getTag(tag)
		--print(tag.." "..data)
		
		i = i+4+flag*2;
		
		res = res..data;
	end
	return res
end

function  getTLV (data,tag)
	local i = 1
	local ftag = ""
	local flen = ""
	local fdata = ""
	while (i < string.len(data) )
	do
		local temp = string.sub(data,i,i+1);
		local itemp = tonumber(temp,16)		
		if ( bitand(itemp,0x1f) == 0x1f ) 
		then  
			ftag = string.sub(data,i,i+3);
			i = i+4;
		else
			ftag = string.sub(data,i,i+1);
			i = i+2;
		end
		
		local lenbyte = string.sub(data,i,i+1);
		if(lenbyte == "81")
		then i = i+2;
		end

		flen =  string.sub(data,i,i+1);
		local iflen = tonumber(flen,16)	
		i = i+2;

		fdata = string.sub(data,i,i+iflen+iflen-1);
		i = i+iflen+iflen
		
		if(ftag == tag)
		then		
			return fdata;
		end
	end
	
	return ""
end


function  ReadAFL (tag_94)
	local i = 1;
	local afl = tag_94;
	local data70 = "";
	while (i < string.len(afl) )
	do
		local sfi = string.sub(afl,i,i+1);
		local snum = string.sub(afl,i+2,i+3);
		local enum = string.sub(afl,i+4,i+5);
		
		local isfi = tonumber(sfi,16);	
		sfi =  string.format("%02X",isfi+4)
		
		local isnum = tonumber(snum,16);	
		local ienum = tonumber(enum,16);	
		
		while (isnum <= ienum )
		do
			local data = Send("00B2"..string.format("%02X",isnum)..sfi.."00","9000");
			isnum = isnum+1;
			
			if((tag8C == "") or (tag8D == "") )
			then			
				data70 = getTLV(data,"70");
				if(tag8C == "" )
				then				
					tag8C = getTLV(data70,"8C");	
				end		
				if(tag8D == "" )
				then					
					tag8D = getTLV(data70,"8D");
				end	
			end			
				
					
		end
		i = i+8;
	end
	
end

function  GPO ()
	local pdolData = MakeDOL(tag9F38);
	
	local pdolDataLen = string.len(pdolData)/2;	
	local data = Send("80A80000"..string.format("%02X",pdolDataLen+2).."83"..string.format("%02X",pdolDataLen)..pdolData ,"9000");
				
	local firstbyte = string.sub(data,1,2); 
	
	if (firstbyte  == "80")
	then
		tag82 = string.sub(data,5,8);	
		tag94 = string.sub(data,9,-5);	
		print("PBOC GPO");
	else  
		data77 = getTLV(data,"77");
		tag82 = getTLV(data77,"82");
		tag94 = getTLV(data77,"94");
		print("qPBOC GPO");	
	end	
	
	ReadAFL(tag94);	
end



function  GAC1 (p1)
	local cdolData =  MakeDOL(tag8C);
	local data = Send("80AE"..p1.."00"..string.format("%02X",string.len(cdolData)/2)..cdolData,"9000");	
	tag9F36 = string.sub(data,7,10);	
	tagAC1 = string.sub(data,11,26);
	--print(tag9F36)
	--print(tagAC1)
end


function  GAC2 (p1)
	local cdolData =  MakeDOL(tag8D);
	Send("80AE"..p1.."00"..string.format("%02X",string.len(cdolData)/2)..cdolData,"9000");	
end


function  VerifyPIN (pin)
	local len = string.len(pin);
	local temp = len..pin.."FFFFFFFFFFFFFFFF";
	temp = string.sub(temp,1,15)
	Send("0020 0080 08 2"..temp,"9000");
end


function  IssuerAuth (arc)
		local in_sk = "000000000000"..tag9F36.."000000000000"..xor(tag9F36,"FFFF");		
		local sk = triDesECBen(in_sk,UDK)
		local data1 = arc.."000000000000"
		
		local d1 = xor(data1,tagAC1)
		local arpc = triDesECBen(d1, sk)
		Send("00820000 0A"..arpc..arc,"9000");
end

function pad(str)
	str = str.."80"
	while(string.len(str)%16 ~=0)
	do
		str = str.."00"
	end	
	return str;
end

function  Script (apdu)
	apdu = string.gsub(apdu," ","");
	local head4 = string.sub(apdu,1,8)
	local data = string.sub(apdu,11,-1)
	local len = string.format("%02X",string.len(data)/2+4)
	local in_sk = "000000000000"..tag9F36.."000000000000"..xor(tag9F36,"FFFF");		
	local sk = triDesECBen(in_sk,MAC_UDK)

	local res = triDesMAC(pad(head4..len..tag9F36..tagAC1..data),sk,"0000000000000000")	
	local mac = string.sub(res,0,8);	
	Send(head4..len..data..mac,"9000")
	
end
