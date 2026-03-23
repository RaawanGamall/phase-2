#!/usr/bin/env python
# coding: utf-8

# In[1]:


import socket
import bluetooth
import time


# In[2]:


soc = socket.socket()

hostname = "localhost"
port = 3333


# In[4]:


TARGET_MAC = xx:xx:xx:xx:xx:xx"  


# In[2]:


soc = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

hostname = "localhost"
port = 3333


# In[3]:


soc.connect((hostname, port))


# In[5]:


while True:
    nearby_devices = bluetooth.discover_devices(lookup_names=True, duration=8)
    print("found %d devices" % len(nearby_devices))

    found = False

    for addr, name in nearby_devices:
        print(" %s - %s" % (addr, name))

        if addr == TARGET_MAC:
            found = True

    if found:
        print("Target device detected!")
        soc.send("OPEN_PAGE1".encode())
    else:
        print("Target device not found")

    time.sleep(5)


# In[ ]:





# In[ ]:




