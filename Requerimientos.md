# Especificación de Requerimientos: Proyecto Vesta

**Estado:** Draft v1.0 | **Fecha:** 2026-02-10  
**Proyecto Maestro:** ARK | **Nombre del Producto:** Vesta  
**Visión:** *La soberanía de la inteligencia en la palma de tu mano.*

---

## 1. Visión General

### 1.1 El Problema: El Invierno de la Privacidad
En la era del software agéntico, las grandes corporaciones han erigido **"Dinosaurios SaaS"** que exigen un tributo inaceptable: el acceso total a la información sensible. Subir un contrato legal a la nube para ser analizado no es solo un riesgo de seguridad; es una cesión de control que alimenta modelos externos mientras debilita la soberanía individual. La extinción de la privacidad es el precio de la conveniencia en el modelo tradicional.

### 1.2 La Solución: El Fuego de Vesta
Vesta nace como un refugio en la **"Zona Verde"**. Es una herramienta de auditoría legal *Offline-First* que utiliza inteligencia local para devolver la confianza al usuario. 
* **No es una renta:** Es una propiedad. 
* **No es un servicio:** Es una utilidad autónoma que garantiza que lo que ocurre en tu dispositivo, muere en tu dispositivo.

---

## 2. Arquitectura de "Islas"
Vesta no es un monolito; es un archipiélago. Siguiendo la filosofía del **Proyecto ARK**, el sistema se basa en el desacoplamiento total mediante Sockets (Interfaces).

* **Aislamiento de Fallos:** Si el motor de IA requiere una actualización, el módulo de OCR o la UI permanecen intactos.
* **Intercambiabilidad:** Permite sustituir a Gemma 2B por modelos futuros sin reescribir la lógica de negocio.
* **Pruebas en Aislamiento:** Cada isla puede ser auditada y testeada sin dependencias externas.

---

## 3. Requerimientos Funcionales (User Stories)

### 3.1 Ingesta y Procesamiento Soberano
* **Historia:** Como usuario, quiero cargar archivos PDF o tomar fotos de contratos físicos para que el sistema extraiga el texto sin necesidad de conexión a internet.
* **Detalle:** Integración de un motor OCR local optimizado para dispositivos móviles.

### 3.2 Auditoría de "Cláusulas de Peligro"
* **Historia:** Como usuario, quiero que la IA identifique automáticamente riesgos ocultos como penalizaciones excesivas, renovaciones automáticas o cláusulas de jurisdicción extranjera.
* **Detalle:** El motor Gemma 2B ejecutará una inferencia semántica sobre el texto extraído buscando patrones legales específicos.

### 3.3 Dashboard de Salud Contractual
* **Historia:** Como usuario, quiero ver un resumen visual rápido del nivel de riesgo de mi documento.
* **Detalle:** Un sistema de semáforo basado en la densidad de hallazgos de la IA:
    * 🔴 **Rojo:** Crítico.
    * 🟡 **Ámbar:** Atención.
    * 🟢 **Verde:** Seguro.

---

## 4. Requerimientos No Funcionales

### 4.1 Privacidad Radical
* **Cero Telemetría:** La aplicación no enviará estadísticas de uso ni logs a servidores externos.
* **Almacenamiento Local:** Todos los documentos y resultados se guardan exclusivamente en el *sandbox* seguro del dispositivo.

### 4.2 Rendimiento de Grado de Producción
* **Latencia de Inferencia:** El análisis de un contrato estándar de 5 páginas debe completarse en menos de 10 segundos.
* **Eficiencia Energética:** Uso optimizado de **ONNX Runtime** para no drenar la batería durante el razonamiento.

### 4.3 Especificaciones de Estilo (C# 10+)
Para garantizar la mantenibilidad por parte del "Orquestador", el código seguirá estas reglas:
1. Declaración de `namespace` con alcance de archivo.
2. Una sola definición de tipo por archivo físico.
3. Comentarios de definición en una sola línea: `///<summary>Misma linea></summary>`.
4. Variables de clase con prefijo underscore: `_variableDeClase`.
5. Llaves de bloque estilo Java (sin inicio de nueva línea).

---

## 5. Roadmap de Desarrollo

| Fase | Título | Descripción |
| :--- | :--- | :--- |
| **Fase I** | Cimientos del Arca | Implementación de Sockets base y configuración de MAUI con ONNX Runtime. |
| **Fase II** | El Ojo de Vesta | Integración de OCR local y carga del modelo Gemma 2B cuantizado. |
| **Fase III** | El Juicio Legal | Desarrollo de la lógica de análisis semántico y dashboard visual. |
| **Fase IV** | Lanzamiento y Libertad | Optimización de UI/UX y despliegue del binario Stand-Alone. |

---

## 6. Estrategia de Monetización: Propiedad Real
Vesta se aleja del modelo de "Renta de Software" de los Dinosaurios SaaS.

* **Modelo:** Pago único (*One-time purchase*).
* **Filosofía:** El usuario compra la herramienta como quien compra un martillo o un libro. Sin suscripciones, sin interrupciones y con propiedad total de la licencia.

> **Nota del Arquitecto:** Vesta no es solo una aplicación; es una declaración de principios. Es la prueba de que la inteligencia no tiene por qué ser una cadena, sino el escudo que nos protege.