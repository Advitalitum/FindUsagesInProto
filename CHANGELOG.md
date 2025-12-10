# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## 0.0.9
Add results ordering, vendor.protogen .proto files will be last

## 0.0.8
FindUsagesInProto now can find OneofCase enums, enum values, properties.

## 0.0.7
FindUsagesInProto now search much faster and will find gRPC generated things in .proto files without csharp_namespace.  

## 0.0.5
FindUsagesInProto now finds gRPC generated classes, it's constructors, properties, enums, enum values, client methods, service methods in .proto files