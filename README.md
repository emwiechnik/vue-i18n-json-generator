This piece of C# code generates JSON file with a format that is expected by the plugin [vue-i18n](https://kazupon.github.io/vue-i18n/).
The input must be in a form of a list of pairs of placeholder and the value.

Example input (the values are provided in a custom object):
```
common.buttons.ok : 'Ok'
common.buttons.cancel : 'Cancel'
admin.dashboard.title: 'Admin Dashboard'
```

... and output:
```javascript
{
 "common": {
   "buttons": {
     "ok": "Ok",
     "cancel": "Cancel"
   }
 },
 "admin": {
   "dadashboard": {
     "title": "Admin Dashboard"
   }
 }
}
```

More context can be found on the [blog post](https://emwiechnik.net/blog/2019/06/23/vue-i18n-and-json-language-files/).
