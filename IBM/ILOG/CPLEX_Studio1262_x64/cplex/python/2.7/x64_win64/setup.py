#!/usr/bin/env python
# ------------------------------------------------------------------------------
# Licensed Materials - Property of IBM
# 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
# Copyright IBM Corporation 2008, 2015. All Rights Reserved.
#
# US Government Users Restricted Rights - Use, duplication or
# disclosure restricted by GSA ADP Schedule Contract with
# IBM Corp.
# ------------------------------------------------------------------------------
"""
setup.py file for the CPLEX Python API
"""

import platform

from distutils.core import setup
from sys import version_info

ERROR_STRING = "CPLEX 12.6.2.0 is not compatible with this version of Python."
SO_FILE = 'py1013_cplex1262.so'

platform_system = platform.system()
if platform_system == 'Darwin':
    if version_info < (2, 7, 0):
        raise Exception(ERROR_STRING)
    elif version_info < (2, 8, 0):
        DATA = [SO_FILE]
    else:
        raise Exception(ERROR_STRING)
elif platform_system in ('Windows', 'Microsoft'):
    if version_info < (2, 7, 0):
        raise Exception(ERROR_STRING)
    elif version_info < (2, 8, 0):
        DATA = ['py27_cplex1262.pyd', 'cplex1262.dll']
    else:
        raise Exception(ERROR_STRING)
elif platform_system in ('Linux', 'AIX'):
    if version_info < (2, 6, 0):
        raise Exception(ERROR_STRING)
    if version_info < (2, 8, 0):
        DATA = [SO_FILE]
    else:
        raise Exception(ERROR_STRING)
else:
    raise Exception("The CPLEX Python API is not supported on this platform.")

setup(name = 'cplex',
      version = '12.6.2.0',
      author = "IBM",
      description = """A Python interface to the CPLEX Callable Library.""",
      packages = ['cplex',
                  'cplex._internal',
                  'cplex.exceptions'],
      package_dir = {'cplex': 'cplex',
                     'cplex._internal': 'cplex/_internal',
                     'cplex.exceptions': 'cplex/exceptions'},
      package_data = {'cplex._internal': DATA},
      url = 'http://www-01.ibm.com/software/websphere/products/optimization/',
      )
