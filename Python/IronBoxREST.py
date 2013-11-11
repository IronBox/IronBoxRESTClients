#------------------------------------------------------------------------
#   IronBox REST API Python wrapper
#   version: 1.1 (11/10/2013)
#   Author: KevinLam@goironbox.com
#   Website: www.goironbox.com
#   Dependencies:
#	python-requests
#	python-openssl
#	python-m2crypto
#------------------------------------------------------------------------
import urllib 
import urllib2 
import os
import json 
import struct
import datetime

# You will need to install python-requests 
# library used in conjuction with urllib2
# Ubuntu: sudo apt-get install python-requests
import requests

# You will need to install openssl, and the M2Crypto wrapper
# Ubuntu: sudo apt-get install python-m2crypto
import M2Crypto

class IronBoxRESTClient():

    # Actual entity identifier, this can be email address,
    # name identifier (mostly internal use only) or an entity
    # ID which is a 64-bit integer that identifies the specific 
    # user
    Entity = "not_defined"

    # Entity type, 0 = email address, 1 = name identifier, 2 = entity ID
    EntityType = 0   

    # Entity password
    EntityPassword = "not_defined"

    # API server URL, default however can be changed
    # to test servers
    APIServerURL = "https://api.goironcloud.com/latest/"

    # Accept format
    ContentFormat = "application/json"

    # Flag that indicates whether or not to be verbose or not
    Verbose = False

    #*************************************************************
    #	IronBox REST Client helper functions
    #*************************************************************

    #-------------------------------------------------------------
    #	Uploads a given file to an IronBox container
    #	
    #	In:
    #	    ContainerID = IronBox container ID, 64-bit integer
    #	    FilePath = local file path of file to upload
    #	    BlobName = name of the file to use on cloud storage
    #	Returns:
    #	    Returns True on success, False otherwise
    #-------------------------------------------------------------
    def UploadFileToContainer(self,ContainerID, FilePath, BlobName):
	BlobIDName = ""
	try:

	    #----------------------------	
	    #	Step 1:
	    #	Test to make sure that the API server is accessible
	    #----------------------------	
	    if self.Ping() is True:
		self.console_log("IronBox API is up, starting transfer")
	    else:
		self.console("IronBox API server is not responding, or is not accessible from this network location")
		raise Exception("IronBox API server is not accessible from this network location") 

	    #----------------------------	
	    #	Step 2:
	    #	Get the container key data
	    #----------------------------	
	    IronBoxKeyData = self.ContainerKeyData(ContainerID)
	    if (IronBoxKeyData is not None):
		self.console_log("Retrieve container symmetric key data")
	    else:
		raise Exception("Unable to retrieve container key data")
	    
	    #----------------------------	
	    #	Step 3:
	    #	Create a container blob and check it out.
	    #	This doesn't actually upload the contents, just 
	    #	creates the entry, and does a "check out" which
	    #	lets IronBox know you're going to upload contents
	    #	soon.  As part of the checkout process you'll get a
	    #	check in token that is your way to check the 
	    #	blob back in.
	    #----------------------------	
	    BlobIDName = self.CreateEntityContainerBlob(ContainerID,BlobName)
	    if BlobIDName is None:
		raise Exception("Unable to create blob in container")
	    CheckOutData = self.CheckOutEntityContainerBlob(ContainerID,BlobIDName) 
	    if CheckOutData is None:
		raise Exception("Unable to checkout container blob")

	    #----------------------------
	    #	Step 4:	
	    #	Make a copy of the file and encrypt it
	    #----------------------------	
	    self.console_log("Encrypting " + FilePath)
	    OriginalFileSize = os.path.getsize(FilePath)
	    EncryptedFilePath = FilePath + ".ironbox"
	    if IronBoxKeyData.Encrypt_File(FilePath,EncryptedFilePath) is False:
		raise Exception("Unable to encrypt local copy of file")

	    #----------------------------	
	    #	Step 5:
	    #	Upload the encrypted file using the shared
	    #	acccess signature we got at checkout 
	    #	Use python-requests, since it's file upload is
	    #	more advanced.
	    #----------------------------	
	    self.console_log("Uploading " + FilePath)
	    url = CheckOutData.SharedAccessSignatureUri
	    encryptedfile = open(EncryptedFilePath, 'rb')
	    headers = {
		'content-type': 'application/octet-stream',
		'x-ms-blob-type' : 'PageBlob'	    # required for cloud blob storage
	    }
	    r = requests.put(url, data=encryptedfile, headers=headers )

	    #----------------------------	
	    #	Step 6:
	    #	Mark the file as ready to download by 
	    #	checking it back in
	    #----------------------------	
	    if self.CheckInEntityContainerBlob(ContainerID,BlobIDName,OriginalFileSize,CheckOutData.CheckInToken) is True:
		self.console_log("Upload completed")
	    else:
		raise Exception("Unable to finalize upload")
	    
	    #done
	    os.remove(EncryptedFilePath) 
	    return True 

	except IOError:
	    # todo: do clean up of file if needed
	    self.console_log("Error reading file")
	    return False

	except Exception, e:
	    # todo: do clean up of file if needed
	    self.console_log(str(e))
	    return False
    
    #-------------------------------------------------------------
    #	Console logger
    #-------------------------------------------------------------
    def console_log(self, m):
	if self.Verbose is True:
	    now = datetime.datetime.now()
	    print str(now) + ": " + m


    #*************************************************************
    #	CORE REST API FUNCTIONS 
    #*************************************************************

    #-------------------------------------------------------------
    #   Checks if the IronBox API server is responding
    #   In:
    #	A URL string to the API server to check
    #   Returns:
    #	A boolean value if 
    #-------------------------------------------------------------
    def Ping(self):
	try:
	    response = urllib2.urlopen(self.APIServerURL + "Ping")
	    responsebody = response.read().lower()
	    response.close()
	    return responsebody in ['true']
	except:
	    return False

    #-------------------------------------------------------------
    #   Fetches an IronBox container key data
    #	
    #	Returns:
    #	    Returns true on success, false otherwise
    #-------------------------------------------------------------
    def ContainerKeyData(self,ContainerID):
	try:
	    postdata = urllib.urlencode({
		'Entity':str(self.Entity),
		'EntityType':str(self.EntityType), 		
		'EntityPassword':self.EntityPassword,
		'ContainerID':str(ContainerID)
	    })
	    url = self.APIServerURL + "ContainerKeyData"

	    # Create and send request
	    request = urllib2.Request(url, postdata, {'Accept': self.ContentFormat})
	    response = urllib2.urlopen(request)
	    responsebody = response.read();
	    response.close()

	    # Parse the response, get container key, IV and strength
	    jsonObj = json.loads(responsebody)
	    #print jsonObj["SessionKeyBase64"]
	    ContainerKeyData = IronBoxKeyData()
	    ContainerKeyData.SymmetricKey = jsonObj["SessionKeyBase64"].decode('base64','strict')
	    ContainerKeyData.IV = jsonObj["SessionIVBase64"].decode('base64','strict')
	    ContainerKeyData.KeyStrength =  int(jsonObj["SymmetricKeyStrength"])

	    #print ContainerKeyData.SymmetricKey.encode('base64','strict') 
	   
	    # No error	
	    return ContainerKeyData 

	except:
	    return None 

  
    #-------------------------------------------------------------
    #	Creates an IronBox blob in an existing container
    #	
    #	Returns:
    #	    Returns the blob ID of the blob created, otherwise
    #	    None on error  
    #-------------------------------------------------------------
    def CreateEntityContainerBlob(self,ContainerID, BlobName):
	try:
	    BlobIDName = None;
	    postdata = urllib.urlencode({
		'Entity':str(self.Entity),
		'EntityType':str(self.EntityType), 		
		'EntityPassword':self.EntityPassword,
		'ContainerID':str(ContainerID),
		'BlobName':BlobName
	    })
	    url = self.APIServerURL + "CreateEntityContainerBlob"

	    # Create and send request
	    request = urllib2.Request(url, postdata, {'Accept': self.ContentFormat})
	    response = urllib2.urlopen(request)
	    BlobID = response.read()
	    BlobIDName = BlobID.strip("\"")
	    #print BlobIDName 
	    response.close()
	    return BlobIDName

	except:
	    return None 


    #-------------------------------------------------------------
    #	Checks outs an entity container blob, so that the caller
    #	can begin uploading the contents of the blob.
    #	
    #	Inputs:
    #	    ContainerID = 64-bit integer container ID
    #	    BlobIDName = ID of the blob being checked out
    #
    #	Returns:
    #	    An IronBoxREST.IronBoxBlobCheckOutData object, 
    #	    otherwise None on error
    #-------------------------------------------------------------
    def CheckOutEntityContainerBlob(self,ContainerID, BlobIDName):
	try:
	    postdata = urllib.urlencode({
		'Entity':str(self.Entity),
		'EntityType':str(self.EntityType), 		
		'EntityPassword':self.EntityPassword,
		'ContainerID':str(ContainerID),
		'BlobIDName':BlobIDName
	    })
	    url = self.APIServerURL + "CheckOutEntityContainerBlob"

	    # Create and send request
	    request = urllib2.Request(url, postdata, {'Accept': self.ContentFormat})
	    response = urllib2.urlopen(request)
	    responseBody = response.read()
	    response.close()

	    # Create a response object
	    jsonObj = json.loads(responseBody)
	    CheckOutData = IronBoxBlobCheckOutData()
	    CheckOutData.SharedAccessSignature = jsonObj["SharedAccessSignature"]
	    CheckOutData.SharedAccessSignatureUri = jsonObj["SharedAccessSignatureUri"]
	    CheckOutData.CheckInToken = jsonObj["CheckInToken"]
	    CheckOutData.StorageUri = jsonObj["StorageUri"]
	    CheckOutData.StorageType = int(jsonObj["StorageType"])
	    CheckOutData.ContainerStorageName = jsonObj["ContainerStorageName"]

	    #CheckOutData.DebugPrintProps()

	    return CheckOutData

	except:
	    return None

    #-------------------------------------------------------------
    #	Checks in a checked out entity container blob
    #
    #	Inputs:	
    #	    ContainerID = 64-bit container ID
    #	    BlobIDName = ID of the blob being checked in
    #	    BlobSizeBytes = Reports the size of the blob in bytes
    #	    CheckInToken = Check in token
    # 
    #-------------------------------------------------------------
    def CheckInEntityContainerBlob(self,ContainerID, BlobIDName, BlobSizeBytes, CheckInToken):
	try:
	    postdata = urllib.urlencode({
		'Entity':str(self.Entity),
		'EntityType':str(self.EntityType), 		
		'EntityPassword':self.EntityPassword,
		'ContainerID':str(ContainerID),
		'BlobIDName':BlobIDName,
		'BlobSizeBytes':str(BlobSizeBytes),
		'BlobCheckInToken':CheckInToken
	    })
	    url = self.APIServerURL + "CheckInEntityContainerBlob"

	    # Create and send request
	    request = urllib2.Request(url, postdata, {'Accept': self.ContentFormat})
	    response = urllib2.urlopen(request)
	    responseBody = response.read().lower()
	    response.close()

	    return responseBody in ['true']

	except:
	    return False

#------------------------------------------------------------------
#   IronBox key data class
#------------------------------------------------------------------
class IronBoxKeyData():

    # Symmetric key
    SymmetricKey = []

    # IV
    IV = []

    # Symmetric key strength 0 = none, 1 = 128 and 2 = 256
    KeyStrength = 2
   
    #-------------------------------------------------------------
    #	Generates the m2crypto name to load
    #-------------------------------------------------------------
    def getm2cryptoname(self):
	Result = "";
	if self.KeyStrength == 2:
	    Result = "aes_256_cbc"
	elif self.KeyStrength == 1:
	    Result = "aes_128_cbc"
	return Result
	    	
    #-------------------------------------------------------------
    #	Encrypts a file using the symmetric key data contained in
    #	in this object
    #	
    #	Returns:
    #	    True on success, false otherwise
    #-------------------------------------------------------------
    def Encrypt_File(self, in_filename, out_filename):
	try:
	    cipher = M2Crypto.EVP.Cipher(self.getm2cryptoname(), self.SymmetricKey, self.IV, op=1)
	    with open(in_filename, 'rb') as infile:
		with open(out_filename, 'wb') as outfile:
		    while True:
			buf = infile.read(1024)
			if not buf:
			    break;
			outfile.write(cipher.update(buf))
		    outfile.write(cipher.final())
		    outfile.close()
		infile.close()	
    
	    return True
	except Exception, e:
	    return False

    #-------------------------------------------------------------
    #	Decrypts a file using the symmetric key data contained in
    #	this object
    #-------------------------------------------------------------
    def Decrypt_File(self, in_filename, out_filename):
	try:
	    cipher = M2Crypto.EVP.Cipher(self.getm2cryptoname(), self.SymmetricKey, self.IV, op=0)
	    with open(in_filename, 'rb') as infile:
		with open(out_filename,'wb') as outfile:
		    while True:
			buf = infile.read(1024)
			if not buf:
			    break
			outfile.write(cipher.update(buf))
		    outfile.write(cipher.final())
		    outfile.close()
		infile.close()
	    return True
	except Exception, e:
	    return False

#------------------------------------------------------------------
#   Class to hold IronBox blob check out data
#------------------------------------------------------------------
class IronBoxBlobCheckOutData():

    SharedAccessSignature = ""
    SharedAccessSignatureUri = ""
    CheckInToken = ""
    StorageUri = ""
    StorageType = 1		# always set to 1
    ContainerStorageName = ""

    def DebugPrintProps(self):
	print "SharedAccessSignature: %s" % self.SharedAccessSignature
	print "SharedAccessSignatureUri: %s" % self.SharedAccessSignatureUri
	print "CheckInToken: %s" % self.CheckInToken 
	print "StorageUri: %s" % self.StorageUri 
	print "StorageType: %d" % self.StorageType 
	print "ContainerStorageName: %s" % self.ContainerStorageName 
    
