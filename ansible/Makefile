.PHONY: prepare site

default: site

prepare:
	gilt overlay

site: prepare
	ansible-playbook -u root -k -D -i site site.yml
