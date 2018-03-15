# Super Remoting Software

This project explores some of the capabilities of the Remoting framework.

The main idea is that we have certain software that manages sock in a store franchise. Each store contains a server that can
be plugged in at any time in a server cluster.

Those servers advertize stock changes and server discovery in a Ring communication bus.
At any time a server may be discovered or sent offline and the Ring must continue to work acting in a decentralized way.
The only time a server needs to know only one of its partners is in startup to join or create a Ring.
