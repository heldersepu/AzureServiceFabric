netsh http delete urlacl url=http://+:8096/
netsh http add urlacl url=http://+:8096/ sddl=D:(A;;GX;;;WD) listen=yes
netsh http delete urlacl url=http://+:8097/
netsh http add urlacl url=http://+:8097/ sddl=D:(A;;GX;;;WD) listen=yes
netsh http delete urlacl url=http://+:8098/
netsh http add urlacl url=http://+:8098/ sddl=D:(A;;GX;;;WD) listen=yes
netsh http delete urlacl url=http://+:8099/
netsh http add urlacl url=http://+:8099/ sddl=D:(A;;GX;;;WD) listen=yes
type NUL > c:\IRan