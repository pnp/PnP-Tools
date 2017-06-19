The MIT License (MIT)

Copyright (c) 2014 Maximilian Melcher

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*** Read Me for real-time Search Query Debugger (v.1.0.0.0) ***

The real-time query debugger tool is a component that runs on a SharePoint Server and parses log queries. 
Those queries are send to the Search Query Tool and displayed there for further analyzing.

** Requirements

In order to get real-time query debugging, the QueryLogger.Monitor must be run on every SharePoint Webserver
 that acts as Query Component (if you are not sure, run it on every server).
The Logging for the SharePoint Search System must be enabled - default settings are sufficient (Central Admin > ).

** Possible Risks

Running the QueryLogger.Monitor on every server can increase the CPU Load on systems under load 
and it can impact the page response time. Additionally the query strings are send unencrypted
 over the network (additonally, but minor load) - if this is a risk for you: dont run it.

** Instructions

1. Copy the zip to all SharePoint Server with the Query      
   Component active - if you dont know what server is     
   responsible copy it to every server - and extract it.
2. Choose one server that acts as host and get its hostname
3. Open the MaxMelcher.QueryLogger.Monitor.exe.config file and  
   set the value StartServer to true and change the ServerUrl to  
   http://{hostname}:{port} - repeat for every SharePoint Server 
   with Query Component on it, but set the StartServer property    
   to false.
4. Start the MaxMelcher.QueryLogger.Monitor.exe host as       
   Administrator 
5. Start all other instances on the Query Component servers

You should see the QueryLogger.Monitor monitoring the ULS Log and printing every line.

** Compile yourself

Starting a compiled .exe file as Administrator is not your thing? Go grab the source here and 
check & compile the code - contributions & feedback are welcome! 
https://github.com/MaxMelcher/QueryLogger

** Disclaimer