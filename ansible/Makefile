.PHONY: prepare site siteroot

default: site

prepare:
	gilt overlay

site: prepare
	ansible-playbook -D -i site site.yml

siteroot: prepare
	ansible-playbook -u root -k -D -i site site.yml
