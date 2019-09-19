STEP 0:

Setup a visual studio Code environment
We Use Visual Studio Code:.
Install VS code 
· Visual Studio Code on win 64
https://code.visualstudio.com/docs/?dv=win64
Or other platform: (https://code.visualstudio.com/docs/setup/setup-overview)
- Install .net Core  SDK:
https://dotnet.microsoft.com/download
https://docs.microsoft.com/en-us/azure/iot-edge/development-environment
· Install VSCode C# extension
	https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp
· Install additional extension  using the VS code ide itself:
	Open VSCODE  and click on extension marketplace 
	
	In the search bar select: 
	 Azure IoT Tools  (it will install Iot workbench and raspberry, Arduino supports too)
		(or use link: https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-toolkit
	 Docker extension 
		 (or use link:  https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)
	 Azure IoT  Hub Toolkit  
	 Azure IoT Edge
 
· Install Docker on windows:
https://docs.docker.com/docker-for-windows/install/

Device Explorer:  very good third party tool to test device, send event check device twin:




-------------------------------------------
DEMO 1 very basic
Cerate C#  solution in VS code  that read sensor data  from Sensirion 
(using  sensrion HHTP server toread data)


0. Register a new device
- Use  Vs code extensionshit ctrl + P :   Azure  IoT Hub Show welcome page
	Follow steps of create IoT Hub   or if HUB 
	When Hub exists  follow steps of register a Device
	Try the device  : Device to cloud message: "Szabolcs"
	Monitor Buil IN event Endpoint
	 


	1. Open VSCODE
	2. Use IoT  Toolkit to connect IoT hub:
	Azure IoT HUB -->…-->Select IoT hub  ( if required  sign in to azure)
	3. If  NO IoT hub:
	Azure IoT HUB -->…-->Create IoT hub  ( if required  sign in to azure) Follow step in VS code
	4. Use VSCode as IoT simulator
	https://devblogs.microsoft.com/iotdev/use-vs-code-as-iot-hub-device-simulator-say-hello-to-azure-iot-hub-in-5-minutes/
	-Show How to Update device twin
Step 2
	1. Use VS Code :Generate code  select C#  to build  a skeleton simulted data project  to send Device to cloud message 
	Go to folder
	Dotnet  restore
	Dotnet run
	Dotnet build
	Publish  to deice:  assumng dotnet core 2.1 already installed
	Dotnet publish -f netcoreapp2.2 -r win10-x64 --self-contained false
	
	Copy publish folder to Device.
	
	Modify this  project  to connect  to sensirion and read real data.Message format : https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-construct

	6. Extend a solution to read sensirion IP from device Twin (clud to device communication)
	See commented lines
	7. Extend Solution to get messages from IoT Callback (direct methods)
	See commented lins
	
	

Lab2: 
Prepare code to IoT  central:  Add the sensirionDataConverter class
Run the code
Copy and pasete one  of the message


Use  IoT central

Login to IoT central free:
https://apps.azureiotcentral.com/


Login to Iot Central:
	Create device template
	Add Measurements:   copy measurement name  coped   previous step.

Iot central based on device provisioning…  
Now we just generate a connection string using a utility
https://docs.microsoft.com/en-us/azure/iot-central/howto-connect-windowsiotcore
Install node js keygen utility.
/ Install node js : https://nodejs.org/en/download/
npm i -g dps-keygen
Genereate key  
dps-keygen -di:f3a0fc71-b690-4dc7-843e-726e1a0e7d6a -dk:xwuWwbA7xCv/bsa4MadwFb7kkKCWFon1+a955DYoYgk= -si:0ne0007B4A5

Use generated key in your device.




Step 3:   IoT Edge development
https://docs.microsoft.com/en-us/azure/iot-edge/how-to-vs-code-develop-module



Machine learnig on the edge:
https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-deploy-machine-learning?context=azure/machine-learning/service/context/ml-context

Anomaly detection with own  model:
https://github.com/Azure/ai-toolkit-iot-edge/tree/master/IoT%20Edge%20anomaly%20detection%20tutorial




Create a c#  console application  that sending messages to the Iot Hub
	
	- Or use az commands:
	
	
	- Or use Azure Portal
	

Mointor Iot hubwith AZ commands:
az iot hub monitor-events --hub-name bsziothub --output table

Create the application:
Right click the  device  and : generatae code:Automatically genearates a DeviceClient  with a sample temp sensor code

Geneareted application is a dotnet core dll  to create an exe  use CLI commands:
Run code in terminal   see readme.md
Dotnet run
Dotnet publish -f netcoreapp2.2 -r win10-x64 --self-contained false


-Extend the connsole  app with Device twin handling




Scenario :  Add a blob storage to the resource group and create message route to save the messages in a blob

Scenario:
Use stream  analytics to compare different sensor  temperature and hummidity value

Technology:  sensirion kit is an arduino device  that communicate on HTTP get


IoT Edge  situation

IoT Edge Marketplace:
Azure Marketplace  / Edge

https://azuremarketplace.microsoft.com/en-us/marketplace/apps/category/internet-of-things?page=1&subcategories=iot-edge-modules



- Deploy Blob storeage on the edge
Documentation: https://docs.microsoft.com/en-us/azure/iot-edge/how-to-deploy-blob
Deploy from Edge module:
	Name - azureblobstorageoniotedge
	Image URI - mcr.microsoft.com/azure-blob-storage:latest
Genarate key for storage: https://generate.plus/en/base64?gp_base64_base%5Blength%5D=64 

Azure IoThub menu:
	- Select IoT hub  (SosIotHub)
	- RightClick Ege  device and  "SetupEdge Simulator"
- Right click on deployment.json file: push and run on simulataor
Notice that Edge wizard put a Simulated sensor into the deployment as defult conainer .. Remove it.




