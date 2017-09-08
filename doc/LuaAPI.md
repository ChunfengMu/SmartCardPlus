for des/3des,the default icv is "0000000000000000"
for sm4/aes,the default icv is "00000000000000000000000000000000"

//xor, xor("11223344","77889900")="66AAAA44"
//return string
xor(string,string)

//xor, bitxor(7,4)=3
//return int
bitxor(int,int)

//and, bitand(7,4)=4
//return int
bitand(int,int)

//or,bitor(7,4)=7
//return int
bitor(int,int)

//reader1 reader2 can constructor two reader ,then lua script can support dual-reader
reader1(readername)
reader2(readername)

//reset card  for dual-reader
reset(readername)  

//send apdu,compare result  for dual-reader
//return apdu's response  
send (apdu, expect, readername)  



//get current dir  
getDir()  


//print  
print(string)  


//print & error count+1  
error(string)  


//send apdu,compare result  
//return apdu's response  
send (apdu, expect)  


//reset card  
reset()  


//aes ecb mode encrypt  
//return result  
aesECBen(plaintext, key)  


//aes ecb mode decrypt  
//return result  
aesECBde(cipher, key)  

//aes cbc mode encrypt  
//return result  
aesCBCen(plaintext, key)  
aesCBCen(plaintext, key, icv) 

//aes cbc mode decrypt  
//return result  
aesCBCde(cipher, key)  
aesCBCde(cipher, key, icv)  


//des ecb mode encrypt  
//return result  
desECBen(plaintext, key)  


//des ecb mode decrypt  
//return result  
desECBde(cipher, key)  

//des cbc mode encrypt  
//return result  
desCBCen(plaintext, key)  
desCBCen(plaintext, key, icv) 

//des cbc mode decrypt  
//return result  
desCBCde(cipher, key)  
desCBCde(cipher, key, icv)  


//return result  
triDesMAC(plaintext, key, icv)  


//triple des ecb mode encrypt  
//return result  
triDesECBen(plaintext, key)  


//triple des ecb mode decrypt  
//return result  
triDesECBde(cipher, key)  


//triple des cbc mode encrypt  
//return result  
triDesCBCen(plaintext, key)  
triDesCBCen(plaintext, key, icv)  


//triple des cbc mode decrypt  
//return result  
triDesCBCde(cipher, key)  
triDesCBCde(cipher, key, icv) 

//hash sha1  
//return result  
sha1(data)  


//rsa encrypt  
//return result  
RSAen(data, n, e)  


//rsa decrypt  
//return result  
RSAde(data, n, d) 


//rsa crt decrypt  
//return result  
RSACRTde(data, p, q, dp, dq, invq)  


//return security domain key dek's session key  
getSKdek()  


//set security domain key  
setKey(enc, mac, dek)  


//init update command  
//return 8050 response  
init(ver)  


//external authenticate command  
//return 8482 response  
ext(sl) 


//according security level send apdu,auto calculate c-mac or c-decryption and c-mac  
//return apdu's response  
secApdu(apdu) 


//SM2Encrypt  C=C1C2C3
//return result   
sm2Enc(sPlaintext, sPubkey)  

//SM2Decrypt  C=C1C2C3
//return result  
sm2De(sCiphertext, sPrikey)  

//SM2Encrypt  C=C1C3C2
//return result   
sm2EncC1C3C2(sPlaintext, sPubkey)  

//SM2Decrypt  C=C1C3C2
//return result  
sm2DeC1C3C2(sCiphertext, sPrikey)  

//SM2Signature  
//return result  
sm2Sign(sPlain, id, sPubKey, sPriKey)  

//SM2Verify  
//return bool  
sm2Verify(sPlain, id, sRS, sPubKey)  

//SM3  
//return result  
sm3(sPlain)  

//SM4EncryptCBC  
//return result  
sm4EncryptCBC(input, iv, key)  

//SM4EncryptECB  
//return result  
sm4EncryptECB(input, key)  

//SM4DecryptCBC  
//return result  
sm4DecryptCBC(input, iv, key)  

//SM4DecryptECB  
//return result  
sm4DecryptECB(input, key)  


//SM4MAC  
//return result  
sm4MAC(input, iv, key)  


