#!/usr/bin/python
#---------------------------------------------------
#   
#   Demonstrates how to upload a file to 
#   an IronBox secure package or container
#
#   Written by KevinLam@goironbox.com 
#   Website: www.goironbox.com
#
#---------------------------------------------------
import string, datetime
from IronBoxREST import IronBoxRESTClient 

#---------------------------------------------------
# Your IronBox authentication parameters, you could
# also pass these in as command arguments
#---------------------------------------------------
ContainerID = 100777	
IronBoxEmail = "email@email.com"
IronBoxPassword = "password"
IronBoxAPIServerURL = "https://api.goironcloud.com/latest/"
InFile = "testfile.txt"
IronBoxFileName = "ironboxfile.txt"

#---------------------------------------------------
# Main
#---------------------------------------------------
def main():

    #---------------------------- 
    #	Create an instance of the IronBox REST class
    #---------------------------- 
    IronBoxRESTObj = IronBoxRESTClient()
    IronBoxRESTObj.Entity = IronBoxEmail
    IronBoxRESTObj.EntityType = 0   # 0 = Entity is email address 
    IronBoxRESTObj.EntityPassword = IronBoxPassword
    IronBoxRESTObj.APIServerURL = IronBoxAPIServerURL
    IronBoxRESTObj.Verbose = True
    
    #---------------------------- 
    #	Upload the file to IronBox	
    #	Duplicate file names will automatically
    #	get renamed
    #---------------------------- 
    IronBoxRESTObj.UploadFileToContainer(ContainerID,InFile,IronBoxFileName)

#---------------------------------------------------
if __name__ == "__main__":
    main()
