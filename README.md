# MultiSense Prototype (VR for Multisensory Experience Research)

## 📖 Project Overview
This repository hosts a Unity-based Virtual Reality (VR) environment designed to investigate **multisensory experiences in open-plan workspaces**. The project is part of Tracy Du’s PhD research at the University of Sydney, focusing on how **visual, auditory, and thermal adjustments** influence user comfort, cognitive performance, and physiological responses.  

The VR prototypes allow participants to immerse themselves in simulated office environments where they can interactively adjust environmental stimuli, while physiological data streams (EEG, GSR, PPG) are synchronised and recorded via the **Lab Streaming Layer (LSL)** framework.  

---

## 🎯 Research Objectives
- Explore how **multisensory environmental design** impacts perceived comfort and work efficiency.  
- Compare user experiences across **single- and multi-modal sensory adjustments** (Visual, Visual + Auditory, Visual + Thermal, Visual + Auditory + Thermal).  
- Integrate **physiological data** (EEG brainwave activity, heart rate, skin conductance) with **cognitive performance tests** to assess user states.  
- Develop a reproducible VR pipeline for **architectural design research** that blends subjective evaluation with objective measurement.  

---

## 🛠 Features
- **Interactive VR Environment**:  
  - Adjust lighting intensity, curtain positions, desk and wall materials.  
  - Enable auditory modifications such as noise masking and dividers.  
  - Control thermal airflow via an Arduino-linked Dyson fan.  

- **Personalisation Options**:  
  - Add functional, personal, and decorative desk items (e.g., dual monitors, anime figure, decorative fan).  
  - Each option influences either visual, auditory, or combined sensory states.  

- **Data Integration**:  
  - Continuous **EEG**, **GSR**, and **PPG** monitoring.  
  - LSL pipeline synchronises data across Emotiv and Shimmer devices.  

- **Cognition Tasks**:  
  - Participants complete VR-based cognitive tests after sensory adjustments to evaluate performance.  

---

## 🚀 Getting Started
### Requirements
- **Unity 2022.3.58 LTS** (URP pipeline recommended)  
- **Git LFS** (for handling large binary assets, e.g., `.fbx`, `.psd`, `.wav`)  
- **VR Hardware** (tested with Galea and Varjo XR-3,PICO 4 Enterprise, META Oculus 2)  
- **Physiological Devices**:  
  - EEG and Heart Rate: *Emotiv MN8 / Flex Cap / Galea*  
  - GSR + PPG: *Shimmer3 GSR+*  
