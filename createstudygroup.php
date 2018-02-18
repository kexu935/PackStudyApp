<?php
	$ini_array = parse_ini_file("path.ini");
  	$path = $ini_array["URL"];
  	$host = $ini_array["HOST"];
  	$password = $ini_array["PASSWORD"];
	$dbusername = $ini_array["USERNAME"];
  	$dname = $ini_array["DNAME"];

  	//connecting to database
 	$conn = mysqli_connect($host, $dbusername, $password, $dname);
	if(mysqli_connect_error())
	{
		die("Could not connect to database");
	}
	$Course = $_POST["Course"];
	$Topic = $_POST["Topic"];
	$UserId = $_POST["UserId"];
	$IdArray = explode(",",$UserId);
	//create table if it does not exist
	$sql = "CREATE TABLE IF NOT EXISTS `{$Course}_{$Topic}` 
    ( `id` INT(11) NOT NULL AUTO_INCREMENT , 
    `UserId` integer DEFAULT 0,
    `Message` TEXT NOT NULL , 
    `Name` TEXT NOT NULL , 
    `reg_date` TIMESTAMP NOT NULL , 
    PRIMARY KEY (`id`)
    ) ENGINE = InnoDB DEFAULT CHARSET=utf8;";
		if ($conn->query($sql) === TRUE) {
		    
		} else {
		    echo "Error creating table: " . $conn->error;
		}
	//loop through and add each study group person
	foreach($IdArray as $id)
    {
      $idInt = (int)$id;
      if($idInt != 0)
      {
		$sql = "INSERT INTO StudyGroups (UserId, ClassId, SessionTitle)
				VALUES ({$idInt}, '{$Course}','{$Topic}')";

		if ($conn->query($sql) === TRUE) {
	    echo "";
		} else {
	    echo "Error: " . $sql . "<br>" . $conn->error;
		}
      }
    }
?>