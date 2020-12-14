-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: rolesusuario
-- ------------------------------------------------------
-- Server version	5.7.18-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `alumno`
--

DROP TABLE IF EXISTS `alumno`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `alumno` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Grupo` varchar(5) NOT NULL,
  `IdMaestro` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fkDocenteAlumno` (`IdMaestro`),
  CONSTRAINT `fkDocenteAlumno` FOREIGN KEY (`IdMaestro`) REFERENCES `maestro` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `alumno`
--

LOCK TABLES `alumno` WRITE;
/*!40000 ALTER TABLE `alumno` DISABLE KEYS */;
INSERT INTO `alumno` VALUES (5,'Nayla Garcia','1.2A',5),(8,'Carlos Lopez','6.1G',3),(9,'David Lopez','1.1P',4),(10,'Mario Castro','1.2A',5),(11,'Mateo Maldonado','6.1G',3),(13,'Carlos Ramos','7.1G',2),(17,'Estefanía Pichardo','6.1G',3),(19,'Sofia Juarez','1.1P',4),(20,'Charlie Martinez','6.2T',1),(21,'Carla Campos','1.2A',5),(22,'Carla Campesina','6.1G',3),(23,'Maria Morales','1.1P',4),(24,'Maro Morales','1.1P',4),(26,'Rosa maria Jaurez','1.1G',7);
/*!40000 ALTER TABLE `alumno` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `director`
--

DROP TABLE IF EXISTS `director`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `director` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Correo` varchar(100) NOT NULL,
  `Contrasena` text NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `director`
--

LOCK TABLES `director` WRITE;
/*!40000 ALTER TABLE `director` DISABLE KEYS */;
INSERT INTO `director` VALUES (1,'Hector Maldonado','correoDirector@hotmail.com','EF7D74898C22A4BF0BE437D30987BC945310DCDF1DDB48A6BCE085F376B302AE');
/*!40000 ALTER TABLE `director` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `maestro`
--

DROP TABLE IF EXISTS `maestro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `maestro` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Correo` varchar(100) NOT NULL,
  `Contrasena` text NOT NULL,
  `Activo` bit(1) NOT NULL DEFAULT b'1',
  `Grupo` varchar(5) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `maestro`
--

LOCK TABLES `maestro` WRITE;
/*!40000 ALTER TABLE `maestro` DISABLE KEYS */;
INSERT INTO `maestro` VALUES (1,'Hector S. Maldonado','saul_99_27@outlook.com','09feef52d2dd3e9c37ff44915264c159c7da95c6e8199711a47d5dda89f068f4','','6.2T'),(2,'Hector Maldonado Gomez','hector_gomez@hotmail.com','09feef52d2dd3e9c37ff44915264c159c7da95c6e8199711a47d5dda89f068f4','','7.1G'),(3,'Esmeralda Lopez','esmelopez@hotmail.com','de2987f25b7773b99674d19f20411b77311dabf54591c49ca8c1a0deb2c7f235','','6.1G'),(4,'Ernestina Leija','ernestina@hotmail.com','b4de838aacff4ad533139c20e994e41e8898923676450c1201e164a87fbbc06d','','1.1P'),(5,'Sebastian San Miguel','sebastian@hotmail.com','4dd68e2ab3a30973318ea903e088b3d3480655ef4236109fe47272c1c1582880','','1.2A'),(6,'Adan Aguilar','adanAguilar@outlook.com','e8105f010c6aeb49f0be49be5930df086ac0971b1ecd5e5f7c9d665b3cccc50b','','1.2T'),(7,'Juan Martinez','juanperez@hotmail.com','ba0073cde6b6089ac4870395aee44c23825d9a02d4261b0bdbc609c7ff9b54e1','','1.1G');
/*!40000 ALTER TABLE `maestro` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-12-13 21:45:17
