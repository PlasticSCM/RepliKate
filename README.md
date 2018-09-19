# RepliKate
Application to replicate a complete Plastic SCM repository.

The code consists of:
* The repliKate itself.  You can use repliKate for two purposes:
  1. **Replicate a complete repository from one server to another**. To replicate a repository from zero, you
     can use the following command to migrate all the source repository content into a new one. All your 
     branches, changesets, permissions, labels and so on will be replicated to the destination repository:
      ```sh
      replikate srcrepos@srcserver dstrepos@dstserver
      ```
     Remember that if you are a GUI guy you can always use the **Sync view** 
     (https://www.plasticscm.com/documentation/gui/plastic-scm-version-control-gui-guide#Chapter23:TheSynchronizationview) 
     to achieve the same result.
   
  2. **Replicate all the changes done inside a repository in a period of time.** To keep your repositories synced,
     you can use the following command: 
      ```sh
      replikate srcrepos@srcserver dstrepos@dstserver --syncdate=<yoursyncdate>
      ```

* RepliKate log. You can use this log4net configuration file to get the log info from the repliKate application,
  you need to place it in the same location as the replikate.exe file, it will generate a replicate.txt log file.


Read this blogpost http://blog.plasticscm.com/2012/02/after-accidentally-cloning-sexy.html to learn how to 
use the RepliKate application.

