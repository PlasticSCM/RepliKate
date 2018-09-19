plastic-repliKate
=================

Application to replicate a complete Plastic SCM repository

The code consists of:
* The repliKate itself.  You can use repliKate for two purposes:
  1. **Replicate a complete repository from one server to another**. To replicate a repository from zero, you can use the following command to migrate all the source repository content into a new one. All your branches, changesets, permissions, labels and so on will be replicated to the destination repository:
      ```sh
      replikate srcrepos@srcserver dstrepos@dstserver
      ```
     Remember that if you are a GUI guy you can always use the **Sync view** (https://www.plasticscm.com/documentation/gui/plastic-scm-version-control-gui-guide#Chapter23:TheSynchronizationview) to achieve the same result.
   
  2. **Replicate all the changes done inside a repository in a period of time.** To keep your repositories synced, you can use the following command: 
      ```sh
      replikate srcrepos@srcserver dstrepos@dstserver --syncdate=<yoursyncdate>
      ```

* RepliKate log. You can use this log4net configuration file to get the log info from the repliKate application, you need to place it in the same location as the replikate.exe file, it will generate a replicate.txt log file.


Read this blogpost http://blog.plasticscm.com/2012/02/after-accidentally-cloning-sexy.html to learn how to use the RepliKate application.
The code consists of:
* The CmdRunner (command runner) itself. This utility allows you to run cm commands from a .NET program,
  thus allowing writing connectors between Plastic SCM and other .NET-based applications. This application
  run cm commands in different process and parses the output obtained. You can run separate commands such as:
  
  > cm find branches
  
  Or within a shell context, such as:
  > cm shell

  > find branches
  
  The main different between both uses is that the cm shell is faster when running several commands in a row,
  since the cm.exe application is already launched. In most cases this is the desired behaviour, since it is
  much better from the performance point of view. The counterpart of using this is that if some configuration
  happens (i.e.: the Plastic SCM server is changed in the client.conf), then the cm shell must be restarted to
  load the new changes.

* CmdRunnerExample: This is an example of a very simple application that executes several common Plastic SCM
  commands and get the result in different ways: get the string result, get the command result (0 means success,
  other value means error). In order to test this example you need to have the "cm.exe" in your path and
  configured with a Plastic server that must be running.

* sampleplasticapi: A sample API for PlasticSCM built using CmdRunner. Many of the basic operations (add,
  check-in, delete, etc.) are implemented. Also, a sample program to demonstrate the API usage is shipped
  along with the actual API code.

* Samples: This project contains the source of all examples displayed in our CmdRunner Guide.
