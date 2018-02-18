<?php
	//get all data necessary for database connection
	$ini_array = parse_ini_file("path.ini");
  	$path = $ini_array["URL"];
  	$host = $ini_array["HOST"];
  	$password = $ini_array["PASSWORD"];
	$dbusername = $ini_array["USERNAME"];
  	$dname = $ini_array["DNAME"];
	
	$uploads_dir = './fileUploads'; //Directory to save the file that comes from client application.
	if ($_FILES["file"]["error"] == UPLOAD_ERR_OK) {
      $tmp_name = $_FILES["file"]["tmp_name"];
      $name = $_FILES["file"]["name"];
      move_uploaded_file($tmp_name, "$uploads_dir/$name");
      echo "sucess";
	}
?>