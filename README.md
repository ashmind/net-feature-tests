### Overview

This project consists of feature tests for main .NET dependency injection framework/libraries.  
It is a follow-up to my old posts on the matter.

### Approach

All documentation/comparison tables are currently generated automatically (based on source code annotations).  
You can see latest version here: [Feature Tables](http://http://diframeworks.apphb.com/).

### Projects

  1. FeatureTests: xUnit feature tests.
  2. FeatureTables.Generator: helper project that is used to generate documentation.  
     Build and run this project after any changes to the tests.
  3. FeatureTables.Web: project that appharbor uses to server the documentation.